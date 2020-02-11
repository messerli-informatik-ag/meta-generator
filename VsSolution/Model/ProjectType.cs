using System;
using System.Collections.Generic;
using System.Linq;

namespace Messerli.VsSolution.Model
{
    public class ProjectType
    {
        // https://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
        private readonly Dictionary<Identifier, Guid> _typeGuids = new Dictionary<Identifier, Guid>
        {
            // https://github.com/dotnet/project-system/issues/1821
            { Identifier.CSharpSdk, Guid.Parse("9A19103F-16F7-4668-BE54-9A1E7A4F7556") },
            { Identifier.CSharpLegacy, Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC") },

            // All other GUIDs
            { Identifier.AspNet5, Guid.Parse("8BB2217D-0F2D-49D1-97BC-3654ED321F3B") },
            { Identifier.AspNetMvc1, Guid.Parse("603C0E0B-DB56-11DC-BE95-000D561079B0") },
            { Identifier.AspNetMvc2, Guid.Parse("F85E285D-A4E0-4152-9332-AB1D724D3325") },
            { Identifier.AspNetMvc3, Guid.Parse("E53F8FEA-EAE0-44A6-8774-FFD645390401") },
            { Identifier.AspNetMvc4, Guid.Parse("E3E379DF-F4C6-4180-9B81-6769533ABE47") },
            { Identifier.AspNetMvc5, Guid.Parse("349C5851-65DF-11DA-9384-00065B846F21") },
            { Identifier.CPlusPlus, Guid.Parse("8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942") },
            { Identifier.Database, Guid.Parse("A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124") },
            { Identifier.DatabaseOther, Guid.Parse("4F174C21-8C12-11D0-8340-0000F80270F8") },
            { Identifier.DeploymentCab, Guid.Parse("3EA9E505-35AC-4774-B492-AD1749C4943A") },
            { Identifier.DeploymentMergeModule, Guid.Parse("06A35CCD-C46D-44D5-987B-CF40FF872267") },
            { Identifier.DeploymentSetup, Guid.Parse("978C614F-708E-4E1A-B201-565925725DBA") },
            { Identifier.DeploymentSmartDeviceCab, Guid.Parse("AB322303-2255-48EF-A496-5904EB18DA55") },
            { Identifier.DistributedSystem, Guid.Parse("F135691A-BF7E-435D-8960-F99683D2D49C") },
            { Identifier.Dynamics2012Ax, Guid.Parse("BF6F8E12-879D-49E7-ADF0-5503146B24B8") },
            { Identifier.FSharp, Guid.Parse("F2A71F9B-5D33-465A-A702-920D77279786") },
            { Identifier.JSharp, Guid.Parse("E6FDF86B-F3D1-11D4-8576-0002A516ECE8") },
            { Identifier.SmartDeviceCSharpLegacy, Guid.Parse("20D4826A-C6FA-45DB-90F4-C717570B9F32") },
            { Identifier.SmartDeviceVbLegacy, Guid.Parse("CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8") },
            { Identifier.MicroFramework, Guid.Parse("b69e3092-b931-443c-abe7-7e7b65f2a37f") },
            { Identifier.MonoAndroid, Guid.Parse("EFBA0AD7-5A72-4C68-AF49-83D382785DCF") },
            { Identifier.MonoTouch, Guid.Parse("6BC8ED88-2882-458C-8E55-DFD12B67127B") },
            { Identifier.MonoTouchBinding, Guid.Parse("F5B4F3BC-B597-4E2B-B552-EF5D8A32436F") },
            { Identifier.PortableClassLibrary, Guid.Parse("786C830F-07A1-408B-BD7F-6EE04809D6DB") },
            { Identifier.ProjectFolders, Guid.Parse("66A26720-8FB5-11D2-AA7E-00C04F688DDE") },
            { Identifier.SharePointCSharp, Guid.Parse("593B0543-81F6-4436-BA1E-4747859CAAE2") },
            { Identifier.SharePointVb, Guid.Parse("EC05E597-79D4-47f3-ADA0-324C4F7C7484") },
            { Identifier.SharePointWorkflow, Guid.Parse("F8810EC1-6754-47FC-A15F-DFABD2E3FA90") },
            { Identifier.SilverLight, Guid.Parse("A1591282-1198-4647-A2B1-27E5FF5F6F3B") },
            { Identifier.SmartDeviceCSharp, Guid.Parse("4D628B5B-2FBC-4AA6-8C16-197242AEB884") },
            { Identifier.SmartDeviceVb, Guid.Parse("68B1623D-7FB9-47D8-8664-7ECEA3297D4F") },
            { Identifier.SolutionFolder, Guid.Parse("2150E333-8FDC-42A3-9474-1A3956D46DE8") },
            { Identifier.Test, Guid.Parse("3AC096D0-A1C2-E12C-1390-A8335801FDAB") },
            { Identifier.UniversalWindowsClassLibrary, Guid.Parse("A5A43C5B-DE2A-4C0C-9213-0A381AF9435A") },
            { Identifier.VbNet, Guid.Parse("F184B08F-C81C-45F6-A57F-5ABD9991F28F") },
            { Identifier.VisualDbTools, Guid.Parse("C252FEB5-A946-4202-B1D4-9916A0590387") },
            { Identifier.Vs2015InstallerProjectExtension, Guid.Parse("54435603-DBB4-11D2-8724-00A0C9A8B90C") },
            { Identifier.VsToolsForApplications, Guid.Parse("A860303F-1F3F-4691-B57E-529FC101A107") },
            { Identifier.VsToolsForOffice, Guid.Parse("BAA0C2D2-18E2-41B9-852F-F413020CAA33") },
            { Identifier.WebApplication, Guid.Parse("349C5851-65DF-11DA-9384-00065B846F21") },
            { Identifier.WebSite, Guid.Parse("E24C65DC-7377-472B-9ABA-BC803B73C61A") },
            { Identifier.Wcf, Guid.Parse("3D9AD99F-2412-4246-B90B-4EAA41C64699") },
            { Identifier.WindowsPhone8Blank, Guid.Parse("76F1466A-8B6D-4E39-A767-685A06062A39") },
            { Identifier.WindowsPhone8CSharp, Guid.Parse("C089C8C0-30E0-4E22-80C0-CE093F111A43") },
            { Identifier.WindowsPhone8Vb, Guid.Parse("DB03555F-0C8B-43BE-9FF9-57896B3C5E56") },
            { Identifier.Wpf, Guid.Parse("60DC8134-EBA5-43B8-BCC9-BB4BC16C2548") },
            { Identifier.WindowsStore, Guid.Parse("BC8A1FFA-BEE3-4634-8014-F334798102B3") },
            { Identifier.WorkflowCSharp, Guid.Parse("14822709-B5A1-4724-98CA-57A101D1B079") },
            { Identifier.WorkflowVb, Guid.Parse("D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8") },
            { Identifier.WorkflowFoundation, Guid.Parse("32F31D43-81CC-4C15-9DE6-3FC5453562B6") },
            { Identifier.XamarinAndroid, Guid.Parse("EFBA0AD7-5A72-4C68-AF49-83D382785DCF") },
            { Identifier.XamarinIos, Guid.Parse("6BC8ED88-2882-458C-8E55-DFD12B67127B") },
            { Identifier.XnaWindows, Guid.Parse("6D335F3A-9D43-41b4-9D22-F6F17C4BE596") },
            { Identifier.XnaXBox, Guid.Parse("2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2") },
            { Identifier.XnaZune, Guid.Parse("D399B71A-8929-442a-A9AC-8BEC78BB2433") },
        };

        public ProjectType(Identifier type)
        {
            Type = type;
        }

        public ProjectType(Guid guid)
        {
            Type = _typeGuids.First(g => g.Value == guid).Key;
        }

        public enum Identifier
        {
            CSharpSdk, // SDK-style projects
            CSharpLegacy,

            AspNet5,
            AspNetMvc1,
            AspNetMvc2,
            AspNetMvc3,
            AspNetMvc4,
            AspNetMvc5,
            CPlusPlus,
            Database,
            DatabaseOther,
            DeploymentCab,
            DeploymentMergeModule,
            DeploymentSetup,
            DeploymentSmartDeviceCab,
            DistributedSystem,
            Dynamics2012Ax,
            FSharp,
            JSharp,
            SmartDeviceCSharpLegacy,
            SmartDeviceVbLegacy,
            MicroFramework,
            MonoAndroid,
            MonoTouch,
            MonoTouchBinding,
            PortableClassLibrary,
            ProjectFolders,
            SharePointCSharp,
            SharePointVb,
            SharePointWorkflow,
            SilverLight,
            SmartDeviceCSharp,
            SmartDeviceVb,
            SolutionFolder,
            Test,
            UniversalWindowsClassLibrary,
            VbNet,
            VisualDbTools,
            Vs2015InstallerProjectExtension,
            VsToolsForApplications,
            VsToolsForOffice,
            WebApplication,
            WebSite,
            Wcf,
            WindowsPhone8Blank,
            WindowsPhone8CSharp,
            WindowsPhone8Vb,
            Wpf,
            WindowsStore,
            WorkflowCSharp,
            WorkflowVb,
            WorkflowFoundation,
            XamarinAndroid,
            XamarinIos,
            XnaWindows,
            XnaXBox,
            XnaZune,
        }

        public Identifier Type { get; }

        public Guid Guid
        {
            get
            {
                if (_typeGuids.ContainsKey(Type))
                {
                    return _typeGuids[Type];
                }

                throw new NotImplementedException("Unknown project type");
            }
        }
    }
}