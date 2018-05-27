﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using GUICommunication.Client;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageServiceGUI.LogTab
{
    class LogModel:ILogModel
    {
        private IGUIClient client;
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void OnLogRecieved(object sender, EventLogEntry entry)
        {
            model_entries.Add(entry);
            NotifyPropertyChanged("entries");
        }

        private ObservableCollection<EventLogEntry> model_entries;
        public ObservableCollection<EventLogEntry> entries
        {
            get
            {
                return model_entries;
            }

            set
            {
                model_entries = value;
                NotifyPropertyChanged("entries");
            }
        }
        public LogModel()
        {
            client = GUIClient.Instance();
            entries = new ObservableCollection<EventLogEntry>();
			client.NewMessage += UpdateLog;
        }

        public void UpdateLog(object sender, string message)
        {
			JObject command = JObject.Parse(message);
			int commandID = (int)command["commandID"];
			// check the commandID for a matching response
			if (commandID == (int)CommandEnum.LogCommand)
			{
				EventLogEntryCollection fromService =
					JsonConvert.DeserializeObject<EventLogEntryCollection>((string)command["LogCollection"]);
				foreach (EventLogEntry entry in fromService)
				{
					entries.Add(entry);
				}
			}
			else if (commandID == (int)CommandEnum.LogUpdate)
			{
				EventLogEntry newLog =
					JsonConvert.DeserializeObject<EventLogEntry>((string)command["Log"]);
				entries.Add(newLog);
			}
		}
    }
}
