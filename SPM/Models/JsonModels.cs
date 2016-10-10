using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPM.JsonModels
{
    class ProgramList
    {
        private string programId;
        private string programName;
        public ProgramList(string programId, string programName)
        {
            this.programId = programId;
            this.programName = programName;
        }
    }
}