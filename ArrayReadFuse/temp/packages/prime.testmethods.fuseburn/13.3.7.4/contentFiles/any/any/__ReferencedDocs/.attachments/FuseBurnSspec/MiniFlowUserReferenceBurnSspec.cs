// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// INTEL CONFIDENTIAL
// Copyright (2019) (2022) Intel Corporation
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

namespace IntegrationTestUserCode.FuseBurn
{
    using System.Collections.Generic;
    using Prime.ConsoleService;
    using Prime.FunctionalService;
    using Prime.FuseDBService;
    using Prime.PatConfigService;
    using Prime.PhAttributes;
    using Prime.PinService;
    using Prime.SessionService;
    using Prime.SharedStorageService;
    using Prime.TestMethods.FuseBurn;

    /// <summary>
    /// Example of plugin. This plugin will mimic default fuse burn sspec execution, ULTEncode and comparing storage value.
    /// </summary>
    [PrimeTestMethod]
    public class MiniFlowUserReferenceBurnSspec : PrimeFuseBurnSspecTestMethod, IFuseBurnSspecExtensions
    {
        private ISessionContextProviderContainer sessionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniFlowUserReferenceBurnSspec"/> class.
        /// </summary>
        /// <param name="consoleService">consoleService.</param>
        /// <param name="pinService">pinService.</param>
        /// <param name="fuseDBService">fuseDBService.</param>
        /// <param name="functionalService">functionalService.</param>
        /// <param name="patConfigService">patConfigService.</param>
        public MiniFlowUserReferenceBurnSspec(IConsoleService consoleService, IPinService pinService, IFuseDBService fuseDBService, IFunctionalService functionalService, IPatConfigService patConfigService)
            : base(consoleService, pinService, fuseDBService, functionalService, patConfigService)
        {
        }

        /// <inheritdoc />
        int IFuseBurnSspecExtensions.ExecuteMiniFlow(IFuseBurnSspecMiniFlow flow)
        {
            this.sessionContext = flow.SessionContext;

            var exitPort = ExecuteMask();

            flow.Datalog($"ExecuteBurnSspec_{flow.RegisterName}_Port_{exitPort}");

            if (exitPort != 1)
            {
                return exitPort;
            }
            else
            {
                exitPort = ExecuteCompareStorage();
            }

            flow.Datalog($"ExecuteCompareStorage_{flow.RegisterName}_Port_{exitPort}");

            return exitPort;

            // Flow 1.
            int ExecuteMask()
            {
                return ExecuteBurnMaskA();
            }

            // Flow 2.
            int ExecuteCompareStorage()
            {
                return CompareSharedStorage() ? CompareUserVar() : flow.ExitPort(3);
            }

            // Flow Item.
            int ExecuteBurnMaskA()
            {
                // Expect onTrue.
                return flow.SspecBurn() ? ExecuteULTEncode() : flow.ExitPort(2);
            }

            // Flow Item.
            int ExecuteULTEncode()
            {
                // Expect onTrue.
                return flow.ULTEncode() ? flow.ExitPort(1) : flow.ExitPort(2);
            }

            // Flow Item.
            bool CompareSharedStorage()
            {
                // Expect Return True
                if (flow.RegisterName.Equals("CPU3"))
                {
                    return flow.CompareSharedStorage($"fusestring_{flow.RegisterName}", Context.DUT, "1011110000001111111");
                }
                else
                {
                    return flow.CompareSharedStorage($"fusestring_{flow.RegisterName}", Context.DUT, "1011110000001111111");
                }
            }

            // Flow Item.
            int CompareUserVar()
            {
                // Expect Return True
                var dieId = "U2U1";
                var isMatch = false;
                var expectedValue = new Dictionary<string, string>()
                {
                    { $"FUSEUFGL.S_{dieId}_FULLBINARY", "00110101001010101010100000110101001010101010100000" },
                    { $"FUSEUFGL.S_{dieId}_GROUPNAME", string.Empty },
                    { $"FUSEUFGL.S_{dieId}_LOT", "Y5106803" },
                    { $"FUSEUFGL.S_{dieId}_WAFER", "330" },
                    { $"FUSEUFGL.S_{dieId}_X", "-10" },
                    { $"FUSEUFGL.S_{dieId}_Y", "+00" },
                    { $"FUSEUFGL.I_{dieId}_WAFER", "330" },
                    { $"FUSEUFGL.I_{dieId}_X", "-10" },
                    { $"FUSEUFGL.I_{dieId}_Y", "0" },
                };

                foreach (var item in expectedValue)
                {
                    isMatch = flow.CompareUservar(item.Key, item.Value);

                    if (!isMatch)
                    {
                        Prime.Base.ServiceStore<IConsoleService>.Service.PrintDebug(() => $"Unmatch Key=[{item.Key}] Value=[{item.Value}]\n", this.sessionContext);
                        break;
                    }
                }

                return isMatch ? flow.ExitPort(1) : flow.ExitPort(4);
            }
        }
    }
}