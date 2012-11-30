using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects.SC;
using CorePoint.DomainObjects;
using ASR.App;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Reflection;

namespace ASR.DomainObjects
{
    [Template("system/asr/command")]
    public class CommandItem : StandardTemplate
    {
        [Field("title")]
        public string Title
        {
            get;
            private set;
        }
        [Field("icon")]
        public string Icon
        {
            get;
            private set;
        }
        [Field("command")]
        public string Command
        { get; set; }

        [Field("singleitemcontext")]
        public bool SingleItemContext
        { get; set;}

        internal void Run(StringList values)
        {
            if (string.IsNullOrEmpty(Command))
            {
                return;
            }

            List<Item> items = new List<Item>();
            StringList othervalues = new StringList();

            foreach (var val in values)
            {
                ItemUri uri = ItemUri.Parse(val);
                if (uri != null)
                {
                    items.Add(Sitecore.Data.Database.GetItem(uri));
                }
                else
                {
                    othervalues.Add(val);
                }
            }

            Command command = CommandManager.GetCommand(Command) 
                ?? (Command)ReflectionUtil.CreateObject(Command);
            Debug.Assert(command != null, Command + " not found.");
            //pass parameters
            var indexSt = Command.IndexOf('(')+1;
            if (indexSt > 0)
            {
                var length = Command.IndexOf(')') - indexSt;
                if(length > 0)
                {
                    ReflectionUtil.SetProperties(command, Command.Substring(indexSt, length));    
                }                
            }

            // If our command can hanlde more than one item in the context we run it once
            if (!SingleItemContext)
            {
                CommandContext cc = new CommandContext(items.ToArray());
                cc.CustomData = othervalues;
                command.Execute(cc);
            }
            //otherwise we have to generate as many commands as items 
            else
            {
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        CommandContext cc = new CommandContext(item);
                        command.Execute(cc);
                    }
                }
                if (othervalues.Count > 0)
                {
                    foreach (var othervalue in othervalues)
                    {
                        CommandContext cc = new CommandContext();
                        cc.CustomData = othervalue;
                        command.Execute(cc);
                    }
                }
            }
        }
    }
}

