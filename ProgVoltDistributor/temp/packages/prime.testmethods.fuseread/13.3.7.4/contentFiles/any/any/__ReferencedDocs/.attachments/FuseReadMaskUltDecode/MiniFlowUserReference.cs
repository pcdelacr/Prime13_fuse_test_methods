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

namespace IntegrationTestUserCode.FuseRead
{
    using System.Collections.Generic;
    using Prime.ConsoleService;
    using Prime.DatalogService;
    using Prime.DffService;
    using Prime.FunctionalService;
    using Prime.PhAttributes;
    using Prime.PinService;
    using Prime.SessionService;
    using Prime.SharedStorageService;
    using Prime.TestMethods.FuseRead;
    using Prime.TestProgramService;
    using Prime.UserVarService;

    /// <summary>
    /// Example of plugin. This plugin will mimic default fuse read mask execution and comparing storage value.
    /// </summary>
    [PrimeTestMethod]
    public class MiniFlowUserReference : PrimeFuseReadMaskUltDecodeTestMethod, IFuseReadMaskUltDecodeExtensions
    {
        private ISessionContextProviderContainer sessionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniFlowUserReference"/> class.
        /// </summary>
        /// <param name="functionalService">functionalService.</param>
        /// <param name="consoleService">consoleService.</param>
        /// <param name="pinService">pinService.</param>
        /// <param name="dffService">dffService.</param>
        /// <param name="datalogService">datalogService.</param>
        /// <param name="userVarService">userVarService.</param>
        /// <param name="testProgramService">testProgramService.</param>
        public MiniFlowUserReference(IFunctionalService functionalService, IConsoleService consoleService, IPinService pinService, IDffService dffService, IDatalogService datalogService, IUserVarService userVarService, ITestProgramService testProgramService)
            : base(functionalService, consoleService, pinService, dffService, datalogService, userVarService, testProgramService)
        {
        }

        /// <inheritdoc />
        int IFuseReadMaskUltDecodeExtensions.ExecuteMiniFlow(IFuseReadMaskMiniFlow flow)
        {
            this.sessionContext = flow.SessionContext;

            var exitPort = ExecuteMaskUlt();

            flow.Datalog($"ExecuteMaskUlt_{flow.RegisterName}_Port_{exitPort}");

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
            int ExecuteMaskUlt()
            {
                return ExecutePassMask();
            }

            // Flow 2.
            int ExecuteCompareStorage()
            {
                return CompareSharedStorage() ? CompareUserVar() : flow.ExitPort(4);
            }

            // Flow Item.
            int ExecutePassMask()
            {
                // Expect onTrue.
                return flow.ReadMask("MaskP1") ? ExecutePassMask2() : flow.ExitPort(2);
            }

            // Flow Item.
            int ExecutePassMask2()
            {
                // Expect onTrue.
                return flow.ReadMask("MaskP2") ? ExecuteFailMask() : flow.ExitPort(2);
            }

            // Flow Item.
            int ExecuteFailMask()
            {
                // Expect onFalse.
                return flow.ReadMask("MaskF1") ? flow.ExitPort(2) : ExecuteFailMask2();
            }

            // Flow Item.
            int ExecuteFailMask2()
            {
                // Expect onFalse.
                return flow.ReadMask("MaskF2") ? flow.ExitPort(2) : ExecuteULTDecode();
            }

            // Flow Item.
            int ExecuteULTDecode()
            {
                // Expect onTrue.
                return flow.ULTDecode() ? flow.ExitPort(1) : flow.ExitPort(3);
            }

            // Flow Item.
            bool CompareSharedStorage()
            {
                // Expect Return True
                if (flow.RegisterName.Equals("CPU3"))
                {
                    return flow.CompareSharedStorage($"fusestring_{flow.RegisterName}", Context.DUT, "00000101010101010010101100000101010101010010101100");
                }
                else
                {
                    return flow.CompareSharedStorage($"fusestring_{flow.RegisterName}", Context.DUT, "01000001010101010010101100000101010101010010101100");
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
                    { $"FUSEUFGL.S_{dieId}_FULLBINARY", "00000101010101010010101100000101010101010010101100" },
                    { $"FUSEUFGL.S_{dieId}_GROUPNAME", string.Empty },
                    { $"FUSEUFGL.S_{dieId}_LOT", "25212990" },
                    { $"FUSEUFGL.S_{dieId}_WAFER", "341" },
                    { $"FUSEUFGL.S_{dieId}_X", "+18" },
                    { $"FUSEUFGL.S_{dieId}_Y", "-12" },
                    { $"FUSEUFGL.I_{dieId}_WAFER", "341" },
                    { $"FUSEUFGL.I_{dieId}_X", "18" },
                    { $"FUSEUFGL.I_{dieId}_Y", "-12" },
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