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

namespace PortControlTEM
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
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 2)]
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 3)]
    [ExitPort(PassFailStatus.Fail, "FAIL PORT", 0)]
    public class PortControlTEM : TestMethodBase
    {
        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Module Name")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Module { get; set; } = "UUC_FUSE";

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("UserVarName")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String UserVarName { get; set; } = "PROG_ENABLE";

        /// <inheritdoc />
        public override void Verify()
        {
        }

        /// <inheritdoc />
        [Returns(1, PortType.Pass, "Pass")]
        public override int Execute()
        {
            // Extract DFF variables
            string prog_var = string.Empty;
            Prime.Base.ServiceStore<IUserVarService>.Service.TryGetValue(this.Module + "::" + this.Module, this.UserVarName, out prog_var);
            if (prog_var == "1")
            {
                return 1;
            }
            else if (prog_var == "2")
            {
                return 2;
            }
            else if (prog_var == "3")
            {
                return 3;
            }
            else
            {
                Prime.Services.ConsoleService.Print("PORTCONTROL_VAR value must be 1, 2 or 3");
                return 0;
            }
        }
    }
}
