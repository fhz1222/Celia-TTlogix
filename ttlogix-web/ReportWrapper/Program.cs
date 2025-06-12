using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json.Linq;
using System;

namespace ReportWrapper
{
    class Program
    {	
		static int Main(string[] args)
        {
            if (args.Length != 10)
            {
                Console.Out.WriteLine("10 arguments required! File Owner Server DB User Pass Title Formula SubreportFormula Params");
                return 1;
            }

			string fileName = args[0];
			string owner = args[1];
			string serverName = args[2];
			string databaseName = args[3];
			string userID = args[4];
			string password = args[5];
			string title = args[6];
			string formula = args[7];
			string subreportFormula = args[8];
			var parameters = JObject.Parse(args[9]);

			var rptDocument = new ReportDocument();
			rptDocument.Load(fileName);
			rptDocument.SummaryInfo.ReportTitle = title;
            if (!string.IsNullOrEmpty(formula))
            {
				rptDocument.RecordSelectionFormula = formula;
            }
            if (!string.IsNullOrEmpty(subreportFormula))
            {
				rptDocument.OpenSubreport("Summary").RecordSelectionFormula = subreportFormula;
            }
			foreach (var param in parameters)
			{
				rptDocument.SetParameterValue(param.Key, param.Value);
			}

			var crConnectionInfo = new ConnectionInfo
            {
                Type = ConnectionInfoType.SQL,
                ServerName = serverName,
                IntegratedSecurity = false,
                DatabaseName = databaseName,
                UserID = userID,
                Password = password
            };

            #region Get/Set Owner Name

            //p_rptDocument.DataDefinition.FormulaFields["OwnerName"].Text = "\"" + l_oOwner.Name +"\"";
            //Change to following code to avoid error raise by report without OwnerName field
            for (int i = 0; i < rptDocument.DataDefinition.FormulaFields.Count; i++)
				if (rptDocument.DataDefinition.FormulaFields[i].FormulaName == "{@OwnerName}")
					rptDocument.DataDefinition.FormulaFields[i].Text = "\"" + owner + "\"";

			#endregion


			//Get the table information from the report
			var crDatabase = rptDocument.Database;
			var crTables = crDatabase.Tables;

			// for each of the tables, apply log on info
			foreach (Table crTable in crTables)
			{
				var crTableLogOnInfo = crTable.LogOnInfo;
				crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
				crTable.ApplyLogOnInfo(crTableLogOnInfo);
				if (crTable.Location.LastIndexOf(".") > 0)
					crTable.Location = crConnectionInfo.DatabaseName + ".dbo." + crTable.Location.Substring(crTable.Location.LastIndexOf(".") + 1);
				//else
					//crTable.Location = crTable.Location;
			}

			// set the sections object to the current report's section 
			var crSections = rptDocument.ReportDefinition.Sections;
			// loop through all the sections to find all the report objects
			foreach (Section crSection in crSections)
			{
				var crReportObjects = crSection.ReportObjects;
				//loop through all the report objects in there to find all subreports
				foreach (ReportObject crReportObject in crReportObjects)
				{
					if (crReportObject.Kind == ReportObjectKind.SubreportObject)
					{
						var crSubreportObject = (SubreportObject)crReportObject;
						//open the subreport object and logon as for the general report
						var crSubreportDocument = crSubreportObject.OpenSubreport(crSubreportObject.SubreportName);
						crDatabase = crSubreportDocument.Database;
						crTables = crDatabase.Tables;
						foreach (Table crTable in crTables)
						{
							var crTableLogOnInfo = crTable.LogOnInfo;
							crTableLogOnInfo.ConnectionInfo = crConnectionInfo;
							crTable.ApplyLogOnInfo(crTableLogOnInfo);
							if (crTable.Location.LastIndexOf(".") > 0)
								crTable.Location = crConnectionInfo.DatabaseName + ".dbo." + crTable.Location.Substring(crTable.Location.LastIndexOf(".") + 1);
							//
								//crTable.Location = crTable.Location;
						}
					}
				}
			}

			try
			{
				rptDocument.ExportToStream(ExportFormatType.PortableDocFormat).CopyTo(Console.OpenStandardOutput());
			}
			catch(Exception e)
            {
				Console.Out.WriteLine(e.Message);
				return 1;
			}
			return 0;
		}		
    }
}
