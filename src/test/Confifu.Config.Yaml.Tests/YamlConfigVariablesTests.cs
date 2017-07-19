using System;
using System.IO;
using Confifu.Config.Yaml.Graph;
using Shouldly;
using Xunit;

namespace Confifu.Config.Yaml.Tests
{
    public sealed class YamlConfigVariablesTests
    {
        [Fact]
        public void it_reads_yaml_property_top_level()
        {
            var vars = new YamlConfigVariables("A: B");
            vars["A"].ShouldBe("B");
        }

        [Fact]
        public void it_returns_null_for_not_existing_keys()
        {
            var vars = new YamlConfigVariables(@"A: B");
            vars["B"].ShouldBeNull();
        }

        [Fact]
        public void it_returns_nested_yaml_property()
        {
            var vars = new YamlConfigVariables(@"
A:
  B: C");

            vars["A:B"].ShouldBe("C");
        }

        [Fact]
        public void it_reads_yaml_from_file()
        {
            var yamlFilePath = Path.GetTempFileName();

            File.WriteAllText(yamlFilePath, @"
A:
  B: C");

            var vars = new YamlConfigVariablesBuilder().UseFile(yamlFilePath, false).Build();

            vars["A:B"].ShouldBe("C");
        }

        [Fact]
        public void it_fails_when_required_file_not_found()
        {
            var yamlFilePath = Path.GetTempFileName();
            File.Delete(yamlFilePath);

            Assert.Throws<InvalidOperationException>(
                () => new YamlConfigVariablesBuilder().UseFile(yamlFilePath, false).Build()
            );
        }


        [Fact]
        public void it_does_not_fail_when_optional_file_not_found()
        {
            var yamlFilePath = Path.GetTempFileName();
            File.Delete(yamlFilePath);

            new YamlConfigVariablesBuilder().UseFile(yamlFilePath, true).Build();
        }

        [Fact]
        public void it_handles_inject_operator()
        {
            var vars = new YamlConfigVariables(@"
dev: &dev
  LogFilePath: log.txt
  Fria2:
    Orm:
      ConnectionString: My dev connection string

smtp_server: &smtp_server sendgrid.com

export:
  <<: *dev
  LogFilePath: \storage.stein-pilz.com\logs\log.txt
  Fria2:
    EmailModule:
      SmtpServer: *smtp_server
");

            vars["export:LogFilePath"].ShouldBe("\\storage.stein-pilz.com\\logs\\log.txt");
            vars["export:Fria2:Orm:ConnectionString"].ShouldBe("My dev connection string");
            vars["export:Fria2:EmailModule:SmtpServer"].ShouldBe("sendgrid.com");
        }

        [Fact]
        public void it_handles_inject_operator_at_top_level()
        {
            var vars = new YamlConfigVariables(@"
R: &R
  A: B

<<: *R
");

            vars["A"].ShouldBe("B");
        }


        [Fact]
        public void it_handles_inject_operators_at_top_level()
        {
            var vars = new YamlConfigVariables(@"
R2: &R2
  A: 2

R1: &R1
  A: 1

<<1: *R1
<<2: *R2
");

            vars["A"].ShouldBe("2");
        }

        [Fact]
        public void it_handles_nested_injections()
        {
            var vars = new YamlConfigVariables(@"
R: &R
  <<: *RR
  S: RS
  T: Q

A:
  <<: *R

RR: &RR
  S: S
");

            vars["A:S"].ShouldBe("RS");
            vars["A:T"].ShouldBe("Q");
            vars["R:T"].ShouldBe("Q");
            vars["R:S"].ShouldBe("RS");
            vars["RR:S"].ShouldBe("S");
        }

        [Fact]
        public void it_handles_test_that_do_not_pass_library_used_for_deserializing()
        {
            // https://github.com/aaubry/YamlDotNet/issues/259
            var vars = new YamlConfigVariables(@"
base: &level1
  X: ""X""

base: &level2
  <<: *level1
  Y: ""Y""

ok:
  <<: *level1

fail:
  <<: *level2
");

            vars["ok:X"].ShouldBe("X");
            vars["fail:X"].ShouldBe("X");
            vars["fail:Y"].ShouldBe("Y");
            vars["base:X"].ShouldBe("X");
            vars["base:Y"].ShouldBe("Y");
        }

        [Fact]
        public void it_handles_parallelogram_correctly()
        {
            var vars = new YamlConfigVariables(@"
A:
  <<1: *B
  <<2: *C

B: &B
  R: 3
  <<: *D

C: &C
  R: 2
  <<: *D

D: &D
  R: 1
");

            vars["D:R"].ShouldBe("1");
            vars["C:R"].ShouldBe("2");
            vars["B:R"].ShouldBe("3");
            vars["A:R"].ShouldBe("2");
        }

        [Fact]
        public void it_throws_on_self_reference()
        {
            // Loop detected: 'R'
            Assert.Throws<ReferenceLoopException>(() =>
                new YamlConfigVariables(@"
R: &R
  A: *R
")
            );
        }

        [Fact]
        public void it_throws_on_loops()
        {
            // Loop detected: 'A', 'A:B', 'A:B:C', 'A'
            Assert.Throws<ReferenceLoopException>(() =>
                new YamlConfigVariables(@"
A: &A
  B:
    C:
      S: *A
")
            );
        }

        [Fact]
        public void it_does_not_throw_on_oriented_graph()
        {
            new YamlConfigVariables(@"
D: &D
  RR: RR
A:
  B:
    S: *D
  C:
    S: *D
");
        }
    }
}
