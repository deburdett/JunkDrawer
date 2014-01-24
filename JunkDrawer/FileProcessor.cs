﻿using System;
using System.Globalization;
using System.IO;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace JunkDrawer
{
    public class FileProcessor
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void Process(FileInformation fileInformation) {

            var p = ProcessFactory.Create("Default");
            var star = fileInformation.ProcessName;
            var table = ("Junk" + star.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0")).PadRight(14, '0');
            var builder = new ProcessBuilder(fileInformation.ProcessName)
                .Star(star)
                .Connection("input").Provider("file").File(fileInformation.FileName).Delimiter(fileInformation.Delimiter).Start(fileInformation.FirstRowIsHeader ? 2 : 1)
                .Connection("output").Server(p.OutputConnection.Server).Database(p.OutputConnection.Database)
                .Entity(table).PrependProcessNameToOutputName(false);

            foreach (var fieldType in fileInformation.FieldTypes()) {
                if (fieldType.Type.Equals("string")) {
                    builder.Field(fieldType.Name).Length(fieldType.Length);
                } else {
                    builder.Field(fieldType.Name).Type(fieldType.Type)
                        .Transform("convert").To(fieldType.Type);
                }
            }

            _log.Info("Process Configuration...", Environment.NewLine);
            _log.Info(Environment.NewLine + builder.Process().Serialize());

            ProcessFactory.Create(builder.Process(), new Options() { Mode = "init" }).Run();
            ProcessFactory.Create(builder.Process(), new Options() { Mode = "default" }).Run();

            File.Move(fileInformation.FileName, fileInformation.FileName + ".bak");
        }
        
    }
}