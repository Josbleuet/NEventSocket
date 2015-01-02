﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayResult.cs" company="Dan Barua">
//   (C) Dan Barua and contributors. Licensed under the Mozilla Public License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NEventSocket.FreeSwitch
{
    using System.IO;

    /// <summary>
    /// Represents the result of the Play dialplan application
    /// </summary>
    public class PlayResult : ApplicationResult
    {
        internal PlayResult(EventMessage eventMessage) : base(eventMessage)
        {
            if (eventMessage != null)
            {
                if (eventMessage.Headers[HeaderNames.ApplicationResponse] == "FILE NOT FOUND")
                {
                    throw new FileNotFoundException(
                        "FreeSwitch was unable to play the file.", eventMessage.Headers[HeaderNames.ApplicationData]);
                }

                this.Success = eventMessage.Headers[HeaderNames.ApplicationResponse] == "FILE PLAYED";
            }
            else
            {
                this.Success = false;
            }
        }
    }
}