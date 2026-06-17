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

namespace DOEsGenAddress1k
{
    using System;
    using System.Linq;
    using System.Reflection;
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
    [ExitPort(PassFailStatus.Pass, "PASS PORT", 2)]
    [ExitPort(PassFailStatus.Fail, "FAIL PORT", 0)]
    public class DOEsGenAddress1k : TestMethodBase
    {
        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert PROG MODE")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Module { get; set; } = "UUC_FUSE";

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert PROG MODE")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Prog_mode { get; set; } = "DATA";

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert PARTITION")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Partition { get; set; } = "00";

        /// <summary>
        /// Gets or sets a string parameter that can be used in your code. You'll populate this
        /// field in your test program's flow definitions (.tpl/.mtpl files).
        /// </summary>
        [HdmtOsTestClassParameter]
        [Description("Insert ARRAY TYPE")]
        [ParameterType(ParameterType.String)]
        [Required]
        public TestMethodsParams.String Array_type { get; set; } = "HVM";

        /// <inheritdoc />
        public override void Verify()
        {
        }

        /// <inheritdoc />
        public override int Execute()
        {
            int execution_count = Prime.Services.UserVarService.GetIntValue(this.Module + "::" + this.Module, "PROG_EXE_COUNT");
            string prog_mode = this.Prog_mode;
            string partition = this.Partition;
            string array_type = this.Array_type;
            int group_rows = 32, array_size = 32, arrays_per_execution = 1;
            int last_exe = 0;
            if (partition == "00" | partition == "01")
            {
                last_exe = 4 / arrays_per_execution;
            }
            else if (partition == "10")
            {
                last_exe = 1 / arrays_per_execution;
            }
            else if (partition == "11")
            {
                last_exe = 2 / arrays_per_execution;
            }
            else if (partition == "100")
            {
                last_exe = 8 / arrays_per_execution;
            }
            else if (partition == "101")
            {
                last_exe = 16 / arrays_per_execution;
            }

            Prime.Services.ConsoleService.Print("Execution count is " + execution_count.ToString());
            if (execution_count == last_exe)
            {
                Prime.Services.ConsoleService.Print("Loop ended");
                Prime.Services.UserVarService.SetValue(this.Module + "::" + this.Module, "PROG_EXE_COUNT", 0);
                return 2;
            }
            else
            {
                int rows_to_program = 0, offset = 0;
                switch (prog_mode)
                {
                    case "DATA":
                        rows_to_program = 32;
                        offset = 0;
                        break;
                    case "SPECIAL":
                        rows_to_program = 8;
                        offset = 32768;
                        break;
                    case "ALL":
                        rows_to_program = 40;
                        array_size = 40;
                        offset = 0;
                        break;
                }

                string temp_str = string.Empty, temp_str_oem = string.Empty, str_row = string.Empty, binary = string.Empty, paddedBinary = string.Empty, reversedBinary = string.Empty;
                int row = 0, address = 0;
                for (int i = 0; i < rows_to_program; i++)
                {
                    row = i;
                    int j = i % group_rows;
                    int k = 2 * array_size / rows_to_program;
                    address = (i * k) - ((k - 1) * j) + (execution_count * arrays_per_execution * 32) + offset;
                    if (array_type == "OEM")
                    {
                        address += 256;
                    }

                    if (i >= 32)
                    {
                        address = ((i - 32) * k) - ((k - 1) * j) + (execution_count * arrays_per_execution * 32) + offset;
                        address += 32768;
                    }

                    binary = Convert.ToString(address, 2);
                    paddedBinary = binary.PadLeft(16, '0');
                    reversedBinary = new string(paddedBinary.Reverse().ToArray());
                    temp_str = string.Concat(reversedBinary.Select(c => new string(c, 1)));
                    str_row = row.ToString();
                    Prime.Services.SharedStorageService.DeleteRowFromTable<string>("ROW_" + row, Prime.SharedStorageService.Context.DUT, this.SessionContext);
                    Prime.Services.SharedStorageService.InsertRowAtTable("ROW_" + row, temp_str, Prime.SharedStorageService.Context.DUT, Prime.SharedStorageService.ResetPolicy.RESET_AT_LOT_START, this.SessionContext);
                    Prime.Services.ConsoleService.Print("ROW_" + row + ": " + temp_str);
                }

                Prime.Services.ConsoleService.Print("Execution " + execution_count.ToString() + " done");
                execution_count++;
                Prime.Services.UserVarService.SetValue(this.Module + "::" + this.Module, "PROG_EXE_COUNT", execution_count);
                return 1;
            }
        }
    }
}