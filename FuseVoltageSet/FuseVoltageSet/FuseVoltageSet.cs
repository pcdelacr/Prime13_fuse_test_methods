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

namespace FuseVoltageSet
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
    public class FuseVoltageSet : TestMethodBase
    {
        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc ehv hvm0")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccEHV_hvm0 { get; set; } = 1.8;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc ehv hvm1")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccEHV_hvm1 { get; set; } = 1.8;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc ehv hvm2")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccEHV_hvm2 { get; set; } = 1.8;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc ehv ifp0")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccEHV_ifp0 { get; set; } = 1.8;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc ehv ifp1")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccEHV_ifp1 { get; set; } = 1.8;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc ehv ifp2")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccEHV_ifp2 { get; set; } = 1.8;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc f nom0")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccFnom0 { get; set; } = 0.7;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc f nom1")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccFnom1 { get; set; } = 0.7;

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc f nom2")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccFnom2 { get; set; } = 0.7;

        /// <summary>
        /// Gets or sets an integer parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files). If not populated
        /// uses the default value.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert vcc core")]
        [ParameterType(ParameterType.Double)]
        [Required]
        public TestMethodsParams.Double Input_VccCore { get; set; } = 0.7;

        /// <inheritdoc />
        public override void Verify()
        {
        }

        /// <inheritdoc />
        public override int Execute()
        {
            double vcchvm0 = this.Input_VccEHV_hvm0;
            double vcchvm1 = this.Input_VccEHV_hvm1;
            double vcchvm2 = this.Input_VccEHV_hvm2;
            double vccifp0 = this.Input_VccEHV_ifp0;
            double vccifp1 = this.Input_VccEHV_ifp1;
            double vccifp2 = this.Input_VccEHV_ifp2;
            double vccfnom0 = this.Input_VccFnom0;
            double vccfnom1 = this.Input_VccFnom1;
            double vccfnom2 = this.Input_VccFnom2;
            double vcccore = this.Input_VccCore;
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM0", vcchvm0);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM1", vcchvm1);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM2", vcchvm2);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP0", vccifp0);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP1", vccifp1);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP2", vccifp2);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM0", vccfnom0);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM1", vccfnom1);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM2", vccfnom2);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_VCCCORE", vcccore);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM0: " + vcchvm0);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM1: " + vcchvm1);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM2: " + vcchvm2);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP0: " + vccifp0);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP1: " + vccifp1);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP2: " + vccifp2);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM0: " + vccfnom0);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM1: " + vccfnom1);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM2: " + vccfnom2);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_VCCCORE: " + vcccore);
            return 1;
        }
    }
}