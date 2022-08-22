using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jeff.Jones.JLogger6;
using Jeff.Jones.JHelpers6;

//// Template code for a method that implements granular logging
//private void SomeMethod(Int32 someVariable)
//{

//	DateTime methodStart = DateTime.Now;

//	// Note that the Logger call is not made unless the bit used for the WriteDebugLog
//	// is on on the m_DebugLogOptions bitmask.  That allows bits to be turned on and off
//	// during operation to adjust what is logged.  No code changes needed to increase or decrease
//	// logging.
//	if ((m_DebugLogOptions & LOG_TYPE.Flow) == LOG_TYPE.Flow)
//	{
//		Logger.Instance.WriteDebugLog(LOG_TYPE.Flow, "1st line in method");
//	}

//	// Declare method-level variables here

//	try
//	{
//		// Do your work here
//	}  // END try
//	catch (Exception exUnhandled)
//	{
//		// I always use a generic catch last if I use multiple catches so I can catch anything that
//		// I did not anticipate
//		exUnhandled.Data.Add("someVariable", someVariable);

//		if ((m_DebugLogOptions & LOG_TYPE.Error) == LOG_TYPE.Error)
//		{
//			Logger.Instance.WriteDebugLog(LOG_TYPE.Error & LOG_TYPE.SendEmail,
//											exUnhandled,
//											"Optional Specific message if desired");
//		}

//	}  // END catch (Exception exUnhandled)
//	finally
//	{
//		// I had no method-level resources here, but if I did, this is how I normally handle them.
//		// BEGIN dispose of method-level resources here =====================================
//		// if (dac != null)
//		//   {
//		//     dac.Dispose();
//		//     dac = null;
//		//   }
//		// END dispose of method-level resources here =======================================

//		if ((m_DebugLogOptions & LOG_TYPE.Performance) == LOG_TYPE.Performance)
//		{
//			TimeSpan elapsedTime = DateTime.Now - methodStart;

//			Logger.Instance.WriteDebugLog(LOG_TYPE.Performance,
//											$"END; elapsed time = [{elapsedTime,0:mm} mins, {elapsedTime,0:ss} secs, {elapsedTime:fff} msecs].",
//											elapsedTime.TotalMilliseconds.ToString());
//		}
//	}  // END finally

//}  // END private void SomeMethod(Int32 someVariable)

namespace LoggingDemo
{
	/// <summary>
	/// The main form for inputting data to test.
	/// </summary>
	public partial class frmMain : Form
	{
		/// <summary>
		/// The debug log bitset for what logging features are on off.
		/// </summary>
		private LOG_TYPE m_DebugLogOptions = LOG_TYPE.Unspecified;

		private String m_EntityName = "Log Testing, Inc.";

		private String m_ErrorMessages = "";
		/// <summary>
		/// Construct the main form for the demo
		/// </summary>
		public frmMain()
		{
			InitializeComponent();

			// Setup logger options here
			m_DebugLogOptions = LOG_TYPE.Error |
								LOG_TYPE.Informational |
								LOG_TYPE.ShowTimeOnly |
								LOG_TYPE.Warning |
								LOG_TYPE.ShowModuleMethodAndLineNumber |
								LOG_TYPE.System |
								LOG_TYPE.SendEmail;

			// Read the folder setting for the file logs
			String filePath = Properties.Settings.Default.LogFolder;

			// If no setting, use the current folder as a default
			if (filePath.Trim().Length == 0)
			{
				filePath = CommonHelpers.CurDir + @"\";
				Properties.Settings.Default.LogFolder = filePath;
				Properties.Settings.Default.Save();
			}

			// Provide ability to pick a path for the file logs.
			dlgFolder.SelectedPath = filePath;

			// Populate the file path textbox with a default value.
			txtLogFolder.Text = filePath;

			// The DB option is not selected by default, so the 
			// data entry for it is also disabled.
			grpDBInfo.Enabled = false;

			// The file option is selected by default, so the 
			// data entry for it is enabled.
			grpFileInfo.Enabled = true;

			// Read in the rest of the settings to the display component.
			txtDBServer.Text = Properties.Settings.Default.DBServer;
			txtDatabase.Text = Properties.Settings.Default.Database;
			chkUseWindowsAuthentication.Checked = Properties.Settings.Default.UseWindowsAuthentication;
			txtUserName.Text = Properties.Settings.Default.DBUserName;
			txtPassword.Text = Properties.Settings.Default.DBPassword;
			txtSMTPServer.Text = Properties.Settings.Default.SMTPServer;
			txtSMTPPort.Text = Properties.Settings.Default.SMTPPort.ToString();
			txtLogonEmail.Text = Properties.Settings.Default.SMTPLogonName;
			txtSMTPLogonPassword.Text = Properties.Settings.Default.SMTPLogonPassword;
			txtReplyToAddress.Text = Properties.Settings.Default.ReplyToAddress;
			txtFromAddress.Text = Properties.Settings.Default.FromAddress;
			txtEntityName.Text = Properties.Settings.Default.EntityName;

			txtAzureStorageDirectory.Text = Properties.Settings.Default.AzureDirectory;
			txtAzureStorageFileShareName.Text = Properties.Settings.Default.AzureFileShare;
			txtAzureStorageResourceID.Text = Properties.Settings.Default.AzureResourceID;

			String sendTo = Properties.Settings.Default.SendToAddresses.Trim();

			// Make the display of the "To address" one per line.
			if (sendTo.Contains(";"))
			{
				sendTo = sendTo.Replace(";", Environment.NewLine);
			}

			if (sendTo.Contains(","))
			{
				sendTo = sendTo.Replace(",", Environment.NewLine);
			}

			txtToAddresses.Text = sendTo;

			txtLogFolder.Text = Properties.Settings.Default.LogFolder;
			txtPrefix.Text = Properties.Settings.Default.LogNamePrefix;


		}

		/// <summary>
		/// If the email bit is chosen, then enable the data entry for sending email.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chkEnableEmail_CheckedChanged(object sender, EventArgs e)
		{
			if (chkEnableEmail.Checked)
			{
				grpEmailInfo.Enabled = true;
			}
			else
			{
				grpEmailInfo.Enabled = false;
			}
		}

		/// <summary>
		/// If the DB option is chosen, then enable the DB data entry
		/// and disable the log file data entry.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void optDB_CheckedChanged(object sender, EventArgs e)
		{
			if (optDB.Checked)
			{
				grpDBInfo.Enabled = true;
				grpFileInfo.Enabled = false;
				chkAzure.Enabled = false;
				chkAzure.Checked = false;
			}
			else
			{
				grpDBInfo.Enabled = false;
				grpFileInfo.Enabled = true;
				chkAzure.Enabled = true;
			}
		}

		/// <summary>
		/// If the file option is chosen, then enable the file data entry
		/// and disable the DB data entry.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void optFile_CheckedChanged(object sender, EventArgs e)
		{
			if (optFile.Checked)
			{
				grpDBInfo.Enabled = false;
				grpFileInfo.Enabled = true;
				chkAzure.Enabled = true;
			}
			else
			{
				grpDBInfo.Enabled = true;
				grpFileInfo.Enabled = false;
				chkAzure.Enabled = false;
			}
		}

		/// <summary>
		/// Run the demo test.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRunTest_Click(object sender, EventArgs e)
		{

			btnRunTest.Enabled = false;

			Boolean response = false;

			// Save the user input to the settings.
			Properties.Settings.Default.DBServer = txtDBServer.Text.Trim();
			Properties.Settings.Default.Database = txtDatabase.Text.Trim();
			Properties.Settings.Default.UseWindowsAuthentication = chkUseWindowsAuthentication.Checked;
			Properties.Settings.Default.DBUserName = txtUserName.Text.Trim();
			Properties.Settings.Default.DBPassword = txtPassword.Text.Trim();
			Properties.Settings.Default.SMTPServer = txtSMTPServer.Text.Trim();
			Properties.Settings.Default.SMTPPort = txtSMTPPort.Text.Trim().GetInt32(587);
			Properties.Settings.Default.SMTPLogonName = txtLogonEmail.Text.Trim();
			Properties.Settings.Default.SMTPLogonPassword = txtSMTPLogonPassword.Text.Trim();
			Properties.Settings.Default.ReplyToAddress = txtReplyToAddress.Text.Trim();
			Properties.Settings.Default.FromAddress = txtFromAddress.Text.Trim();
			Properties.Settings.Default.EntityName = txtEntityName.Text.Trim();

			Properties.Settings.Default.AzureDirectory = txtAzureStorageDirectory.Text.Trim();
			Properties.Settings.Default.AzureFileShare = txtAzureStorageFileShareName.Text.Trim();
			Properties.Settings.Default.AzureResourceID = txtAzureStorageResourceID.Text.Trim();


			String sendTo = txtToAddresses.Text.Trim();

			if (sendTo.Contains(";"))
			{
				sendTo = sendTo.Replace(";", Environment.NewLine);
			}

			if (sendTo.Contains(","))
			{
				sendTo = sendTo.Replace(",", Environment.NewLine);
			}

			Properties.Settings.Default.SendToAddresses = sendTo;
			Properties.Settings.Default.LogFolder = txtLogFolder.Text.Trim();
			Properties.Settings.Default.LogNamePrefix = txtPrefix.Text.Trim();
			Properties.Settings.Default.Save();

			// Get the values for the test.
			String filePath = txtLogFolder.Text.Trim();
			String fileNamePrefix = txtPrefix.Text.Trim();
			Int32 daysToRetainLogs = Convert.ToInt32(numDaysToRetain.Value);

			// Calculate the debug options bitset from what is checked.
			GetDebugLogOptions();

			// Is this a DB or file check?
			if (optDB.Checked)
			{
				String dbServer = txtDBServer.Text.Trim();
				String dbDatabase = txtDatabase.Text.Trim();
				Boolean useWindowsAuth = chkUseWindowsAuthentication.Checked;
				String dbUserName = txtUserName.Text.Trim();
				String dbPassword = txtPassword.Text.Trim();

				// Set the db configuration.  Because it is a singleton,
				// the instance does not have to be explicitly instantiated.
				// Just set the configs needed, and start the log as shown below.
				// An "entity name" can be used to differentiate an oprganization or
				// function related to the log entry.  This sets the default entity name.
				// and each log write allows a different entity name.
				response = Logger.Instance.SetDBConfiguration(dbServer,
																dbUserName,
																dbPassword,
																useWindowsAuth,
																true,
																dbDatabase,
																daysToRetainLogs,
																m_DebugLogOptions);

			}
			else
			{
				// Set the log file configuration.  Because it is a singleton,
				// the instance does not have to be explicitly instantiated.
				// Just set the configs needed, and start the log as shown below.
				// An "entity name" can be used to differentiate an oprganization or
				// function related to the log entry.  This sets the default entity name.
				// and each log write allows a different entity name.
				response = Logger.Instance.SetLogData(filePath,
										  fileNamePrefix,
										  daysToRetainLogs,
										  m_DebugLogOptions,
										  "");

				Logger.Instance.DBEnabled = false;

				if (chkAzure.Checked)
				{

					String azureStorageResourceID = txtAzureStorageResourceID.Text.Trim();
					String azureStorageFileShareName = txtAzureStorageFileShareName.Text.Trim();
					String azureStorageDirectory = txtAzureStorageDirectory.Text.Trim();
					Boolean useAzureFileStorage = true;

					response = Logger.Instance.SetAzureConfiguration(azureStorageResourceID,
																	 azureStorageFileShareName,
																	 azureStorageDirectory,
																	 useAzureFileStorage);

				}
				else
				{
					Logger.Instance.UseAzureFileStorage = false;
				}
			}


			if (chkEnableEmail.Checked)
			{
				// Gather the config data for sending email.
				Int32 smtpPort = Convert.ToInt32(txtSMTPPort.Text);
				Boolean useSSL = chkUseSSL.Checked;
				List<String> sendToAddresses = txtToAddresses.Text.Split(new[] { Environment.NewLine },
																		 StringSplitOptions.None).ToList<String>();


				String smtpServerName = txtSMTPServer.Text.Trim();
				String smtpLogonEmail = txtLogonEmail.Text.Trim();
				String smtpPassword = txtSMTPLogonPassword.Text.Trim();
				String smtpFromAddress = txtFromAddress.Text.Trim();
				String smtpReplyToAddrsss = txtReplyToAddress.Text.Trim();

				// Set the configuration for sending email, if sending email is desired.
				response = Logger.Instance.SetEmailData(smtpServerName,
													smtpLogonEmail,
													smtpPassword,
													smtpPort,
													sendToAddresses,
													smtpFromAddress,
													smtpReplyToAddrsss,
													useSSL);
			}

			// Now that the desired configs are set, start the log.
			Boolean result = Logger.Instance.StartLog();

			if (!result)
			{
				MessageBox.Show(this, "Log Start Failed");
			}

			m_ErrorMessages = "";

			// For the demo, we simulate multithreaded use with the Parallel.For
			//  The log will show the difference ThreadIDs used.
			Parallel.For(1, 101, i =>
			{
				TestMethod(i);
			});

			if (m_ErrorMessages.Length > 0)
			{
				MessageBox.Show(this, m_ErrorMessages);
			}

			// Now that the testing is over, stop the log.
			result = Logger.Instance.StopLog();

			if (!result)
			{
				MessageBox.Show(this, "Log Stop Failed");
			}

			// Dispose releases resources, and will also do a StopLog if not already done.
			Logger.Instance.Dispose();

			// Enable the button so the test can be run again with different values.
			btnRunTest.Enabled = true;

			m_ErrorMessages = "";
		}

		/// <summary>
		/// This method is used to perform test debug log writes.
		/// </summary>
		/// <param name="counter">The loop iteration number from the Parallel.For loop.</param>
		private void TestMethod(Int32 counter)
		{

			Boolean result = Logger.Instance.WriteDebugLog(LOG_TYPE.Flow,
										  "1st line in method",
										  "");

			DateTime methodStart = DateTime.Now;

			if (!result)
			{
				if (!m_ErrorMessages.Contains("Flow bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
				{
					m_ErrorMessages += "Flow bit not enabled." + Environment.NewLine;
				}
			}

			//// auditUserName and auditWorkstation are optional, and only used when usong the 
			//// DB option, and then only when using the DB schema with the audit log DB (DBLogAudit)
			//// enabled.
			//String auditUserName = "";
			//String auditWorkstation = "";

			//if (lblAuditTable.Visible)
			//{
			//	auditUserName = Environment.UserDomainName + @"\" + Environment.UserName;
			//	auditWorkstation = Environment.MachineName;
			//}

			// Note that the Logger call is not made unless the bit used for the WriteDebugLog
			// is on on the m_DebugLogOptions bitmask.  That allows bits to be turned on and off
			// during operation to adjust what is logged.  No code changes needed to increase or decrease
			// logging.  By checking the bit (a low resource, fast check), we save the method call
			// overhead for when the entry would not be logged.
			if ((m_DebugLogOptions & LOG_TYPE.Flow) == LOG_TYPE.Flow)
			{
				// For demo purposes, send an email only for the first log entry.
				if (counter == 1)
				{
					result = Logger.Instance.WriteDebugLog(LOG_TYPE.Flow | LOG_TYPE.SendEmail,
												  "1st line in method",
												  "");
				}
				else
				{
					result = Logger.Instance.WriteDebugLog(LOG_TYPE.Flow,
												  "1st line in method",
												  "");
				}

				if (!result)
				{
					if (!m_ErrorMessages.Contains("Flow bit not enabled. 1st line.", StringComparison.CurrentCultureIgnoreCase))
					{
						m_ErrorMessages += "Flow bit not enabled. 1st line." + Environment.NewLine;
					}
				}
			}

			try
			{
				// A simple divide-by-zero exception is created and logged.
				Int32 x = 100;

				if ((m_DebugLogOptions & LOG_TYPE.Test) == LOG_TYPE.Test)
				{
					result = Logger.Instance.WriteDebugLog(LOG_TYPE.Test,
												 "Test Message",
												 "More info here");

					if (!result)
					{
						if (!m_ErrorMessages.Contains("Test bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
						{
							m_ErrorMessages += "Test bit not enabled." + Environment.NewLine;
						}
					}
				}

				Int32 y = 5 - 5;

				if ((m_DebugLogOptions & LOG_TYPE.System) == LOG_TYPE.System)
				{
					result = Logger.Instance.WriteDebugLog(LOG_TYPE.System,
												  "TickCount",
												  Environment.TickCount.ToString());

					if (!result)
					{
						if (!m_ErrorMessages.Contains("System bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
						{
							m_ErrorMessages += "System bit not enabled." + Environment.NewLine;
						}
					}
				}

				Double Z = x / y;

				if ((m_DebugLogOptions & LOG_TYPE.Cloud) == LOG_TYPE.Cloud)
				{
					Logger.Instance.WriteDebugLog(LOG_TYPE.Cloud,
												  "Cloud Message",
												  $"Task Counter value = {counter.ToString()}");

					if (!result)
					{
						if (!m_ErrorMessages.Contains("Cloud bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
						{
							m_ErrorMessages += "Cloud bit not enabled." + Environment.NewLine;
						}
					}
				}

			}
			catch (DivideByZeroException exDiv)
			{
				// The logger captures the data added to the excpetion instance's Data collection.
				exDiv.Data.Add("x", 100);
				exDiv.Data.Add("y", 0);

				if ((m_DebugLogOptions & LOG_TYPE.Error) == LOG_TYPE.Error)
				{
					// For demo purposes, send an email only for the first log entry.
					if (counter == 1)
					{
						result = Logger.Instance.WriteDebugLog(LOG_TYPE.Error | LOG_TYPE.SendEmail,
													  exDiv,
													  "Division by zero was intentional");
					}
					else
					{
						result = Logger.Instance.WriteDebugLog(LOG_TYPE.Error,
														exDiv,
														"Division by zero was intentional");
					}

					if (!result)
					{
						if (!m_ErrorMessages.Contains("Error bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
						{
							m_ErrorMessages += "Error bit not enabled." + Environment.NewLine;
						}
					}
				}

			}
			catch (Exception exUnhandled)
			{
				// I always use a generic catch last so I can catch anything that
				// I did not anticipate
				exUnhandled.Data.Add("x", 100);
				exUnhandled.Data.Add("y", 0);

				if ((m_DebugLogOptions & LOG_TYPE.Error) == LOG_TYPE.Error)
				{
					result = Logger.Instance.WriteDebugLog(LOG_TYPE.Error & LOG_TYPE.SendEmail,
												  exUnhandled,
												  "Division by zero was intentional");

					if (!result)
					{
						if (!m_ErrorMessages.Contains("Send Email bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
						{
							m_ErrorMessages += "Send Email bit not enabled." + Environment.NewLine;
						}
					}
				}

			}
			finally
			{
				// I had no method-level resources here, but if I did, this is how I normally handle them.
				// BEGIN dispose of method-level resources here =====================================
				// if (dac != null)
				//   {
				//     dac.Dispose();
				//     dac = null;
				//   }
				// END dispose of method-level resources here =======================================

				if ((m_DebugLogOptions & LOG_TYPE.Performance) == LOG_TYPE.Performance)
				{
					TimeSpan elapsedTime = DateTime.Now - methodStart;

					result = Logger.Instance.WriteDebugLog(LOG_TYPE.Performance,
												  $"END; elapsed time = [{elapsedTime,0:mm} mins, {elapsedTime,0:ss} secs, {elapsedTime:fff} msecs].",
												  elapsedTime.TotalMilliseconds.ToString());

					if (!result)
					{
						if (!m_ErrorMessages.Contains("Performance bit not enabled.", StringComparison.CurrentCultureIgnoreCase))
						{
							m_ErrorMessages += "Performance bit not enabled." + Environment.NewLine;
						}
					}
				}
			}

		}

		/// <summary>
		/// Reads the debug log option checkboxes, and builds the m_DebugLogOptions bitmask to use for logging.
		/// </summary>
		private void GetDebugLogOptions()
		{

			m_DebugLogOptions = LOG_TYPE.Unspecified;

			if (chkFlow.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Flow;
			}

			if (chkError.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Error;
			}

			if (chkInformational.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Informational;
			}

			if (chkWarning.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Warning;
			}

			if (chkSystem.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.System;
			}

			if (chkPerformance.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Performance;
			}

			if (chkTest.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Test;
			}

			if (chkEnableEmail.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.SendEmail;
			}

			if (chkDatabase.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Database;
			}

			if (chkService.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Service;
			}

			if (chkCloud.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Cloud;
			}

			if (chkManagement.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Management;
			}

			if (chkFatal.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Fatal;
			}

			if (chkNetwork.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Network;
			}

			if (chkThreat.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Threat;
			}

			if (chkIncludeMethod.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.ShowModuleMethodAndLineNumber;
			}

			if (chkTimeOnly.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.ShowTimeOnly;
			}

			if (chkFlow.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.Flow;
			}

			if (chkHideThreadID.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.HideThreadID;
			}

			if (chkIncludeStackTrace.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.IncludeStackTrace;
			}

			if (chkIncludeExceptionData.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.IncludeExceptionData;
			}

			if (chkSendEmail.Checked)
			{
				m_DebugLogOptions |= LOG_TYPE.SendEmail;
			}

			Logger.Instance.DebugLogOptions = m_DebugLogOptions;

		}

		/// <summary>
		/// Used to call the folder select UI to choose the folder for written log files.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnFolder_Click(object sender, EventArgs e)
		{
			DialogResult result = dlgFolder.ShowDialog(this);

			if (result == DialogResult.OK)
			{
				txtLogFolder.Text = dlgFolder.SelectedPath;
			}

		}

		/// <summary>
		/// Method called by the button to validate the DB exists, can be connected to, and whether or 
		/// not the DBLogAudit table is present.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnValidateDB_Click(object sender, EventArgs e)
		{

			DataAccessLayer dac = null;

			try
			{
				// Read the DB-related user input.
				String DBServer = txtDBServer.Text.Trim();
				String Database = txtDatabase.Text.Trim();
				Boolean UseWindowsAuthentication = chkUseWindowsAuthentication.Checked;
				String DBUserName = txtUserName.Text.Trim();
				String DBPassword = txtPassword.Text.Trim();

				// Create an instance of a generic data access layer just for this 
				// logger demo.
				dac = new DataAccessLayer(DBServer,
										  Database,
										  UseWindowsAuthentication,
										  DBUserName,
										  DBPassword,
										  10,
										  10);

				// Assume the DBLogAudit table does not exist until we know it does.
				Boolean isAudit = false;

				// Make sure the DB connection works.
				Boolean connOK = dac.CheckConnection(out isAudit);

				// Update the indicators to show if the DB connection worked or not 
				// and if the 
				lblDBOK.Visible = true;

				if (connOK)
				{
					lblDBOK.BackColor = Color.Green;
				}
				else
				{
					lblDBOK.BackColor = Color.Red;
				}

				//lblAuditTable.Visible = true;

				if (isAudit)
				{
					lblAuditTable.Text = "Audit Table Present";
				}
				else
				{
					lblAuditTable.Text = "No Audit Table";
				}


			}
			catch (Exception exUnhandled)
			{
				// Catch and display any errors in this method.
				lblDBOK.BackColor = Color.Red;
				lblAuditTable.Visible = false;
				MessageBox.Show(this, $"Error in DataAccessLayer{Environment.NewLine}{exUnhandled.GetFullExceptionMessage(false, false)}", "Database Connection Issue");
			}
			finally
			{
				// Do what Momma taught you - clean up after yourself. :)
				dac = null;
			}

		}

		/// <summary>
		/// Method for button to add a text string as a UDF field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUDFAdd_Click(object sender, EventArgs e)
		{
			if (txtUDF.Text.Trim().Length > 0)
			{
				if (!lstUDF.Items.Contains(txtUDF.Text.Trim()))
				{
					lstUDF.Items.Add(txtUDF.Text.Trim());
				}

				txtUDF.Text = "";
			}
		}

		/// <summary>
		/// Method for button to remove a text string as a UDF field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUDFRemove_Click(object sender, EventArgs e)
		{
			if (lstUDF.Items.Count > 0)
			{
				List<String> toRemove = new List<String>();

				foreach (String item in lstUDF.SelectedItems)
				{
					toRemove.Add(item);
				}

				foreach (String item in toRemove)
				{
					lstUDF.Items.Remove(item);
				}
			}
		}

		private void chkAzure_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAzure.Checked)
			{
				lblResourceID.Visible = true;
				txtAzureStorageResourceID.Visible = true;

				lblFileShare.Visible = true;
				txtAzureStorageFileShareName.Visible = true;

				lblDirectory.Visible = true;
				txtAzureStorageDirectory.Visible = true;
			}
			else
			{
				lblResourceID.Visible = false;
				txtAzureStorageResourceID.Visible = false;

				lblFileShare.Visible = false;
				txtAzureStorageFileShareName.Visible = false;

				lblDirectory.Visible = false;
				txtAzureStorageDirectory.Visible = false;
			}
		}
	}  // public partial class frmMain : Form

}  // END namespace LoggingDemo
