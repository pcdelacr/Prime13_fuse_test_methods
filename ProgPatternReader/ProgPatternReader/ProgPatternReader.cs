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

namespace ProgPatternReader
{
    using HdmtOs.Sdk.TestClass.BaseTypes.Attributes;
    using Prime;
    using Prime.ConsoleService;
    using Prime.PhAttributes;
    using Prime.TestMethods;

    /// <summary>
    /// The description of this test method.
    /// </summary>
    [PrimeTestMethod]
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 1)]
    [ExitPort(PassFailStatus.Fail, "FAIL PORT", 0)]
    public class ProgPatternReader : TestMethodBase
    {
        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyParam description.")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String MyParam { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an integer parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files). If not populated
        /// uses the default value.
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("MyOptionalParam description.")]
        [ParameterType(ParameterType.Integer)]
        [DefaultValue(1)]
        public TestMethodsParams.Integer MyOptionalParam { get; set; } = 1;

        /// <inheritdoc />
        public override void Verify()
        {
            Prime.Base.ServiceStore<IConsoleService>.Service.Print("Starting Verify...");
            if (this.MyParam == string.Empty)
            {
                throw new Prime.Base.Exceptions.TestMethodException("I've failed you! :'(");
            }
        }

        /// <inheritdoc />
        public override int Execute()
        {
            return -1;
        }
    }
}