﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoofLab.VKBot.Core.Commands
{
    public record ReminderCommand(DateTime ReminderDate, string ReminderText) : Command
    {

    }
}
