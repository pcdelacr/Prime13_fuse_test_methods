// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// INTEL CONFIDENTIAL
// Copyright (2019) (2021) Intel Corporation
//
// The source code contained or described herein and all documents related to the source code ("Material") are
// owned by Intel Corporation or its suppliers or licensors. Title to the Material remains with Intel Corporation
// or its suppliers and licensors. The Material contains trade secrets and proprietary and confidential
// information of Intel Corporation or its suppliers and licensors. The Material is protected by worldwide copyright
// and trade secret laws and treaty provisions. No part of the Material may be used, copied, reproduced, modified,
// published, uploaded, posted, transmitted, distributed, or disclosed in any way without Intel Corporation's prior express
// written permission.
//
// No license under any patent, copyright, trade secret or other intellectual property right is granted to or
// conferred upon you by disclosure or delivery of the Materials, either expressly, by implication, inducement,
// estoppel or otherwise. Any license under such intellectual property rights must be express and approved by
// Intel in writing.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace DFFVoltageExtract
{
    using HdmtOs.Sdk.TestClass.BaseTypes.Attributes;
    using Prime;
    using Prime.ConsoleService;
    using Prime.PhAttributes;
    using Prime.TestMethods;
    using Prime.UserVarService;

    /// <summary>
    /// The description of this test method.
    /// </summary>
    [PrimeTestMethod]
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 1)]
    [ExitPort(PassFailStatus.Fail, "FAIL PORT", 0)]
    public class DFFVoltageExtract : TestMethodBase
    {
        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Module { get; set; } = "UUC_FUSE";

        /// <inheritdoc />
        public override void Verify()
        {
        }

        /// <inheritdoc />
        [Returns(1, PortType.Pass, "Pass")]
        public override int Execute()
        {
            // Extract DFF variables
            string dff_vccf_nom0, dff_vccf_nom1, dff_vccf_nom2, dff_vcc_ehv_hvm0, dff_vcc_ehv_hvm1, dff_vcc_ehv_hvm2, dff_vcc_ehv_ifp0, dff_vcc_ehv_ifp1, dff_vcc_ehv_ifp2, dff_vcc_core = string.Empty;
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCF_NOM0", out dff_vccf_nom0);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCF_NOM1", out dff_vccf_nom1);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCF_NOM2", out dff_vccf_nom2);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCEHV_HVM0", out dff_vcc_ehv_hvm0);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCEHV_HVM1", out dff_vcc_ehv_hvm1);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCEHV_HVM2", out dff_vcc_ehv_hvm2);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCEHV_IFP0", out dff_vcc_ehv_ifp0);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCEHV_IFP1", out dff_vcc_ehv_ifp1);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_PROG_VCCEHV_IFP2", out dff_vcc_ehv_ifp2);
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "GDFF_FUSE_VCCCORE", out dff_vcc_core);

            // Set voltages
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM0", double.Parse(dff_vccf_nom0));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM1", double.Parse(dff_vccf_nom1));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM2", double.Parse(dff_vccf_nom2));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM0", double.Parse(dff_vcc_ehv_hvm0));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM1", double.Parse(dff_vcc_ehv_hvm1));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM2", double.Parse(dff_vcc_ehv_hvm2));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP0", double.Parse(dff_vcc_ehv_ifp0));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP1", double.Parse(dff_vcc_ehv_ifp1));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP2", double.Parse(dff_vcc_ehv_ifp2));
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_VCCCORE", double.Parse(dff_vcc_core));
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM0: " + dff_vccf_nom0);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM1: " + dff_vccf_nom1);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM2: " + dff_vccf_nom2);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM0: " + dff_vcc_ehv_hvm0);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM1: " + dff_vcc_ehv_hvm1);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM2: " + dff_vcc_ehv_hvm2);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP0: " + dff_vcc_ehv_ifp0);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP1: " + dff_vcc_ehv_ifp1);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP2: " + dff_vcc_ehv_ifp2);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_VCCCORE: " + dff_vcc_core);
            return 1;
        }
    }
}
