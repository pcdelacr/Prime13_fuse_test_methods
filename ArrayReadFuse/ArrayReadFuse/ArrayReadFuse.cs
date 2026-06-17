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

namespace ArrayReadFuse
{
    using System.Runtime.CompilerServices;
    using HdmtOs.Sdk.TestClass.BaseTypes.Attributes;
    using Prime;
    using Prime.ConsoleService;
    using Prime.PhAttributes;
    using Prime.SessionService;
    using Prime.SharedStorageService;
    using Prime.TestMethods.Callbacks;

    /// <summary>
    /// The description of this test method.
    /// </summary>
    [PrimeTestMethod]
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 1)]
    [ExitPort(PassFailStatus.Fail, "FAIL PORT", 0)]
    public class ArrayReadFuse : PrimeCallbacksRegistrarTestMethod
    {
        /// <summary>
        /// validate array sense conditions in the ctv results.
        /// </summary>
        /// <param name="args">Will have the syntax NUMARRAYS,MODULE,reset.</param>
        public static void FuseArrayCheck(string args)
        {
            if (args.Split(':')[2] == "reset")
            {
                Prime.Services.UserVarService.SetValue(args.Split(':')[1] + "::" + args.Split(':')[1], "ARRAY_FAIL_COUNTER", 1);
                return;
            }

            string temp_row_result = string.Empty, pass_flag = string.Empty;
            int row_fail_counter = 0, arrays = 4;
            arrays = int.Parse(args.Split(':')[0]);
            for (int arr = 0; arr < arrays; arr++)
            {
                for (int row = 0; row < 42; row++)
                {
                    temp_row_result = Prime.Services.SharedStorageService.GetRowFromTable<string>("FUSE_A" + arr.ToString() + "_R" + row.ToString(), Context.DUT, Services.SessionService.GetCurrentThreadSessionContextContainer());
                    if (temp_row_result != "00000000000000000000000000000000000000000000")
                    {
                        row_fail_counter++;
                    }
                }
            }

            int array_fail_counter = 0;
            array_fail_counter = Prime.Services.UserVarService.GetIntValue(args.Split(':')[1] + "::" + args.Split(':')[1], "ARRAY_FAIL_COUNTER");
            if (row_fail_counter != 0)
            {
                Prime.Services.ConsoleService.PrintDebug(() => $"<info> Fuse Array Check failed", Prime.Base.ServiceStore<ISessionService>.Service.GetCurrentThreadSessionContextContainer());
                array_fail_counter++;
                Prime.Services.UserVarService.SetValue(args.Split(':')[1] + "::" + args.Split(':')[1], "ARRAY_FAIL_COUNTER", 2);
                return;
            }
            else
            {
                Prime.Services.ConsoleService.PrintDebug(() => $"<info> Fuse Array Check passed", Prime.Base.ServiceStore<ISessionService>.Service.GetCurrentThreadSessionContextContainer());
                return;
            }
        }

        /// <inheritdoc />
        public override void RegisterCallbacks()
        {
            this.RegisterCallback(FuseArrayCheck);
        }
    }
}
