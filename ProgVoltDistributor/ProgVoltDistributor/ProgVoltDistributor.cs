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

namespace ProgVoltDistributor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using HdmtOs.Sdk.TestClass.BaseTypes.Attributes;
    using Prime;
    using Prime.ConsoleService;
    using Prime.PhAttributes;
    using Prime.PlatformService.Internal;
    using Prime.SharedStorageService;
    using Prime.TestMethods;
    using Prime.UserVarService;

    /// <summary>
    /// The description of this test method.
    /// </summary>
    [PrimeTestMethod]
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 1)]
    [ExitPort(PassFailStatus.Fail, "FAIL PORT", 0)]
    public class ProgVoltDistributor : TestMethodBase
    {
        private List<double> ivcccore = new () { 0.70 };
        private List<double> ivccfnom0 = new () { 0.70 };
        private List<double> ivccfnom1 = new () { 0.70 };
        private List<double> ivccfnom2 = new () { 0.70 };
        private List<double> iehvhvm0 = new () { 1.80 };
        private List<double> iehvhvm1 = new () { 1.80 };
        private List<double> iehvhvm2 = new () { 1.80 };
        private List<double> iehvifp0 = new () { 1.80 };
        private List<double> iehvifp1 = new () { 1.80 };
        private List<double> iehvifp2 = new () { 1.80 };

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Module { get; set; } = "UUC_FUSE";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_vcccore { get; set; } = "0.7";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_vccfnom0 { get; set; } = "0.7";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_vccfnom1 { get; set; } = "0.7";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_vccfnom2 { get; set; } = "0.7";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_ehv_hvm0 { get; set; } = "1.8";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_ehv_hvm1 { get; set; } = "1.8";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_ehv_hvm2 { get; set; } = "1.8";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_ehv_ifp0 { get; set; } = "1.8";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_ehv_ifp1 { get; set; } = "1.8";

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.CommaSeparatedDouble I_ehv_ifp2 { get; set; } = "1.8";

        /// <inheritdoc />
        public override void Verify()
        {
        }

        /// <inheritdoc />
        [Returns(1, PortType.Pass, "Pass")]
        public override int Execute()
        {
            int counter = 0;
            bool found = Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, "PROG_COUNT_UNITS", out counter);
            string vid = Prime.Base.ServiceStore<IUserVarService>.Service.GetScValue("SC_VISUALID");
            if (!found)
            {
                Prime.Services.ConsoleService.Print("user var: " + this.Module + "::" + this.Module + ".PROG_COUNT_UNITS not found");
                return 0;
            }
            else
            {
                Prime.Services.ConsoleService.Print(this.Module + "::" + this.Module + ".PROG_COUNT_UNITS: " + counter.ToString());
            }

            this.ivcccore = this.I_vcccore.ToList();
            this.ivccfnom0 = this.I_vccfnom0.ToList();
            this.ivccfnom1 = this.I_vccfnom1.ToList();
            this.ivccfnom2 = this.I_vccfnom2.ToList();
            this.iehvhvm0 = this.I_ehv_hvm0.ToList();
            this.iehvhvm1 = this.I_ehv_hvm1.ToList();
            this.iehvhvm2 = this.I_ehv_hvm2.ToList();
            this.iehvifp0 = this.I_ehv_ifp0.ToList();
            this.iehvifp1 = this.I_ehv_ifp1.ToList();
            this.iehvifp2 = this.I_ehv_ifp2.ToList();
            int divider = this.ivcccore.Count();
            int progvoltindex = counter % divider;
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM0", this.ivccfnom0[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM1", this.ivccfnom1[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCF_NOM2", this.ivccfnom2[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM0", this.iehvhvm0[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM1", this.iehvhvm1[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_HVM2", this.iehvhvm2[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP0", this.iehvifp0[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP1", this.iehvifp1[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_PROG_VCCEHV_IFP2", this.iehvifp2[progvoltindex]);
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue("SCVars.FUSE_VCCCORE", this.ivcccore[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM0: " + this.ivccfnom0[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM1: " + this.ivccfnom1[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCF_NOM2: " + this.ivccfnom2[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM0: " + this.iehvhvm0[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM1: " + this.iehvhvm1[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_HVM2: " + this.iehvhvm2[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP0: " + this.iehvifp0[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP1: " + this.iehvifp1[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_PROG_VCCEHV_IFP2: " + this.iehvifp2[progvoltindex]);
            Prime.Services.ConsoleService.Print("SCVars.FUSE_VCCCORE: " + this.ivcccore[progvoltindex]);
            Prime.SessionService.ISessionContextProviderContainer contxt = Prime.Services.SessionService.GetCurrentThreadSessionContextContainer();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt1 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt2 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt3 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt4 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt5 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt6 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt7 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt8 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt9 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            Prime.DatalogService.DatalogSpec.IStrgvalFormat comnt10 = Prime.Services.DatalogService.GetItuffStrgvalWriter();
            comnt1.ForceItuffLevel("2");
            comnt2.ForceItuffLevel("2");
            comnt3.ForceItuffLevel("2");
            comnt4.ForceItuffLevel("2");
            comnt5.ForceItuffLevel("2");
            comnt6.ForceItuffLevel("2");
            comnt7.ForceItuffLevel("2");
            comnt8.ForceItuffLevel("2");
            comnt9.ForceItuffLevel("2");
            comnt10.ForceItuffLevel("2");
            comnt1.SetData("SCVars.FUSE_PROG_VCCF_NOM0:" + this.ivccfnom0[progvoltindex]);
            comnt2.SetData("SCVars.FUSE_PROG_VCCF_NOM1:" + this.ivccfnom1[progvoltindex]);
            comnt3.SetData("SCVars.FUSE_PROG_VCCF_NOM1:" + this.ivccfnom2[progvoltindex]);
            comnt4.SetData("SCVars.FUSE_PROG_VCCEHV_HVM0:" + this.iehvhvm0[progvoltindex]);
            comnt5.SetData("SCVars.FUSE_PROG_VCCEHV_HVM1:" + this.iehvhvm1[progvoltindex]);
            comnt6.SetData("SCVars.FUSE_PROG_VCCEHV_HVM2:" + this.iehvhvm2[progvoltindex]);
            comnt7.SetData("SCVars.FUSE_PROG_VCCEHV_IFP0:" + this.iehvifp0[progvoltindex]);
            comnt8.SetData("SCVars.FUSE_PROG_VCCEHV_IFP1:" + this.iehvifp1[progvoltindex]);
            comnt9.SetData("SCVars.FUSE_PROG_VCCEHV_IFP2:" + this.iehvifp2[progvoltindex]);
            comnt10.SetData("SCVars.FUSE_VCCCORE:" + this.ivcccore[progvoltindex]);
            Prime.Services.DatalogService.WriteToItuff(comnt1, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt2, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt3, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt4, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt5, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt6, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt7, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt8, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt9, contxt);
            Prime.Services.DatalogService.WriteToItuff(comnt10, contxt);
            counter++;
            Prime.Base.ServiceStore<IUserVarService>.Service.SetValue(this.Module + "::" + this.Module + ".PROG_COUNT_UNITS", counter);
            Prime.Services.ConsoleService.Print(this.Module + "::" + this.Module + ".PROG_COUNT_UNITS: " + counter.ToString());
            return 1;
        }
    }
}