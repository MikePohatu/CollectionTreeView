﻿namespace viewmodel
{
    public class SccmPackageProgram : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.PackageProgram; } }
        public string PackageName { get; set; }
        public string PackageID { get; set; }
        public string ProgramName { get; set; }

        /// <summary>
        /// This is the string representation of the dependent program mapping. This maps to the 
        /// same property in the SMS_Program query table 
        /// </summary>
        public string DependentProgram { get; set; }
        public string Description { get; set; }
        public SccmPackageProgram DependentSccmPackageProgram { get; set; }
        public SccmPackage ParentPackage { get; set; }
    }
}
