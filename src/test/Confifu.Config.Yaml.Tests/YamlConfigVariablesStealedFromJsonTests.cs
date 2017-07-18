using System;
using System.IO;
using Shouldly;
using Xunit;

namespace Confifu.Config.Yaml.Tests
{
    public sealed class YamlConfigVariablesStealedFromJsonTests
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
  Fria2: # здесь я не уверен, что Fria2 не перепишет весь объект и Orm: ConnectionString пропадет
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
    }
}
