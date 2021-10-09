using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AEIEditor
{
	public static class ConsoleManager
	{
		public static void ShowError(string msg, bool addHelp = true)
		{
			if (addHelp)
			{
				UiManager.ShowMessage(msg + "\r\nCommand format: AEIEditor Action [Parameters separated by spaces]\r\n\r\nAction:\r\n    Help                        -   show this text,\r\n                                        all parameters are ignored\r\n    Create FilePNG FileAEI      -   create AEI file from PNG texture,\r\n                                        the following parameters are ignored::\r\n                                            TexturePath,\r\n                                            TextureMap\r\n    Change FileAEI              -   save AEI file\r\n                                        parameter TextureMap are ignored\r\n    Save FileAEI NewFileAEI   -   save new AEI file\r\n                                        parameter TextureMap are ignored\r\n    Export FileAEI FilePNG      -   save PNG texture from AEI file,\r\n                                        all parameters are ignored\r\n    ImportTextures FileAEI Path -   import texture fragments from PNG\r\n                                        files at the specified absolute path,\r\n                                        all in alphabetical order,\r\n                                        or bound by TextureMap\r\n    ExportTextures FileAEI Путь -   export texture fragments to PNG\r\n                                        files at the specified absolute path,\r\n                                        names are set according to the scheme:\r\n                                            TextureNumber.AEIFileName\r\n                                        or bound by TextureMap,\r\n                                        all other parameters are ignored\r\n\r\nParameters and their default values:\r\n\r\n    Silent=false    -   true if the program should not require\r\n                            user actions and display an interface\r\n    TexturePath=    -   Texture file to replace\r\n    Format=RAW      -   Compression format, options:\r\n                                RAW            -   None\r\n                                RAW_UI         -   None, interface\r\n                                RAW_CubeMap_PC -   None, CubeMap(PC)\r\n                                RAW_CubeMap    -   None, CubeMap\r\n                                DXT1\r\n                                DXT3\r\n                                DXT5\r\n                                ETC1           -   ETC1 no transparency\r\n\t                            PVRTCI2A       -   PVRTC1 2bpp\r\n                                PVRTCI4A       -   PVRTC1 4bpp\r\n                                ATC            -   ATC 4bpp\r\n    MipMapped=false -   Use mip-mapping\r\n    Quality=3       -   Compressed texture quality from 0 to 3\r\n    TextureMap=     -   Binding a file name, separated by commas\r\n                                to texture numbers, through a colon, for example:\r\n                                texA.png:1,texB.png:2,texC.png:3\r\n    BatchMode=false -   Batch mode.\r\n                            Instead of file names, you need to specify folders\r\n                            for working with files in them.\r\n                            Not supported:\r\n                                Operation ImportTextures\r\n                                Parameter TextureMap\r\n\r\nIf you need to specify a path or file name with spaces, use\r\ndouble quotes", "Error starting from command line");
				return;
			}
			UiManager.ShowMessage(msg, "Error starting from command line");
		}

		public static void ShowHelp()
		{
			Console.WriteLine("\r\nCommand format: AEIEditor Action [Parameters separated by spaces]\r\n\r\nAction:\r\n    Help                        -   show this text,\r\n                                        all parameters are ignored\r\n    Create FilePNG FileAEI      -   create AEI file from PNG texture,\r\n                                        the following parameters are ignored::\r\n                                            TexturePath,\r\n                                            TextureMap\r\n    Change FileAEI              -   save AEI file\r\n                                        parameter TextureMap are ignored\r\n    Save FileAEI NewFileAEI   -   save new AEI file\r\n                                        parameter TextureMap are ignored\r\n    Export FileAEI FilePNG      -   save PNG texture from AEI file,\r\n                                        all parameters are ignored\r\n    ImportTextures FileAEI Path -   import texture fragments from PNG\r\n                                        files at the specified absolute path,\r\n                                        all in alphabetical order,\r\n                                        or bound by TextureMap\r\n    ExportTextures FileAEI Путь -   export texture fragments to PNG\r\n                                        files at the specified absolute path,\r\n                                        names are set according to the scheme:\r\n                                            TextureNumber.AEIFileName\r\n                                        or bound by TextureMap,\r\n                                        all other parameters are ignored\r\n\r\nParameters and their default values:\r\n\r\n    Silent=false    -   true if the program should not require\r\n                            user actions and display an interface\r\n    TexturePath=    -   Texture file to replace\r\n    Format=RAW      -   Compression format, options:\r\n                                RAW            -   None\r\n                                RAW_UI         -   None, interface\r\n                                RAW_CubeMap_PC -   None, CubeMap(PC)\r\n                                RAW_CubeMap    -   None, CubeMap\r\n                                DXT1\r\n                                DXT3\r\n                                DXT5\r\n                                ETC1           -   ETC1 no transparency\r\n\t                            PVRTCI2A       -   PVRTC1 2bpp\r\n                                PVRTCI4A       -   PVRTC1 4bpp\r\n                                ATC            -   ATC 4bpp\r\n    MipMapped=false -   Use mip-mapping\r\n    Quality=3       -   Compressed texture quality from 0 to 3\r\n    TextureMap=     -   Binding a file name, separated by commas\r\n                                to texture numbers, through a colon, for example:\r\n                                texA.png:1,texB.png:2,texC.png:3\r\n    BatchMode=false -   Batch mode.\r\n                            Instead of file names, you need to specify folders\r\n                            for working with files in them.\r\n                            Not supported:\r\n                                Operation ImportTextures\r\n                                Parameter TextureMap\r\n\r\nIf you need to specify a path or file name with spaces, use\r\ndouble quotes");
		}

		private static ConsoleResult PreParseParams(string[] args, int actionArgCount, out bool batchMode, out string texturePath)
		{
			texturePath = null;
			batchMode = false;
			for (var i = actionArgCount + 1; i < args.Length; i++)
			{
				var array = args[i].Split('=');
				var num = array.Length;
				if (num > 1)
				{
					var param = array[0];
					var text = array[num - 1];
					var num2 = Array.FindIndex(_paramNames, a => string.Compare(a, param, true) == 0);
					if (num2 < 0)
					{
						ShowError("Unknown parameter: " + param);
						return ConsoleResult.ErrorParam;
					}
					var num3 = num2;
					switch (num3)
					{
					case 0:
					{
						var flag = false;
						if (!bool.TryParse(text, out flag))
						{
							ShowError("Could not recognize parameter value " + param);
							return ConsoleResult.ErrorSilent;
						}
						UiManager.ShowUi = !flag;
						break;
					}
					case 1:
						texturePath = text;
						break;
					default:
						if (num3 == 6)
						{
							if (!bool.TryParse(text, out batchMode))
							{
								ShowError("Could not recognize parameter value " + param);
								return ConsoleResult.ErrorBatchMode;
							}
						}
						break;
					}
				}
			}
			return ConsoleResult.Ok;
		}

		private static ConsoleResult ParseParams(string[] args, int actionArgCount, AeScene scene, bool textureMapOnly, bool isExportAction, out KeyValuePair<int, string>[] textureMap)
		{
			textureMap = null;
			for (var i = actionArgCount + 1; i < args.Length; i++)
			{
				var array = args[i].Split('=');
				var num = array.Length;
				if (num > 1)
				{
					var param = array[0];
					var text = array[num - 1];
					var num2 = Array.FindIndex(_paramNames, a => string.Compare(a, param, true) == 0);
					if (num2 < 0)
					{
						ShowError("Unknown parameter: " + param);
						return ConsoleResult.ErrorParam;
					}
					if (!textureMapOnly || num2 == 5)
					{
						switch (num2)
						{
						case 2:
						{
							var format = Compression.FormatFromArgument(text);
							if (format == Compression.Ae2Format.Unknown)
							{
								ShowError("Unknown parameter value " + param);
								return ConsoleResult.ErrorFormat;
							}
							scene.GetResource().CompressionAe2Format = format;
							break;
						}
						case 3:
						{
							var mipMapped = false;
							if (!bool.TryParse(text, out mipMapped))
							{
								ShowError("Could not recognize parameter value " + param);
								return ConsoleResult.ErrorMipMapped;
							}
							if (!scene.SetMipMapped(mipMapped))
							{
								ShowError("Compression format does not support mip-mapping, skipping parameter " + param, false);
							}
							break;
						}
						case 4:
						{
							byte b;
							if (!byte.TryParse(text, out b))
							{
								ShowError("Could not recognize parameter value " + param);
								return ConsoleResult.ErrorQuality;
							}
							if (b > 3)
							{
								ShowError("Invalid parameter value " + param);
								return ConsoleResult.ErrorWrongQuality;
							}
							scene.GetResource().Quality = b;
							break;
						}
						case 5:
						{
							var textureCount = scene.GetTextureCount();
							var array2 = text.Split(',');
							var num3 = array2.Length;
							textureMap = new KeyValuePair<int, string>[num3];
							for (var j = 0; j < num3; j++)
							{
								var array3 = array2[j].Split(':');
								var num4 = array3.Length;
								var flag = false;
								if (num4 > 1)
								{
									int num5;
									if (!int.TryParse(array3[num4 - 1], out num5))
									{
										flag = true;
									}
									else
									{
										if (num5 > textureCount)
										{
											ShowError(string.Concat("Invalid value in parameter ", param, ": texture number(", num5.ToString(), ")is greater than what is in the AEI file(", textureCount.ToString(), ")"));
											return ConsoleResult.ErrorTextureMapId;
										}
										var text2 = Program.ValidateFilePath(array3[0], ".png", false, isExportAction);
										if (text2 == null)
										{
											return ConsoleResult.ErrorInvalidPath;
										}
										textureMap[j] = new KeyValuePair<int, string>(num5 - 1, text2);
									}
								}
								if (flag)
								{
									ShowError("Could not recognize parameter value " + param);
									return ConsoleResult.ErrorTextureMap;
								}
							}
							break;
						}
						}
					}
				}
			}
			return ConsoleResult.Ok;
		}

		private static ConsoleResult DoAction(string[] args, int actionArgCount, int actionId, string filePath1, string filePath2, string texturePath, AeScene scene)
		{
			var flag = actionId == 4;
			var flag2 = actionId == 6;
			if (actionId == 1)
			{
				var text = Program.ValidateFilePath(filePath1, ".png");
				var text2 = Program.ValidateFilePath(filePath2, ".aei", false, true);
				if (text == null || text2 == null)
				{
					return ConsoleResult.ErrorInvalidPath;
				}
				scene.Load(new AeImage(null, new Bitmap(text), text2));
			}
			else
			{
				var text3 = Program.ValidateFilePath(filePath1, ".aei");
				if (text3 == null)
				{
					return ConsoleResult.ErrorInvalidPath;
				}
				scene.LoadResource(text3);
			}
			if (texturePath != null)
			{
				scene.ImportBitmap(Program.ValidateFilePath(texturePath, ".png"));
			}
			if (flag)
			{
				scene.ExportBitmap(Program.ValidateFilePath(filePath2, ".png", false, true));
			}
			else
			{
				if (actionId == 3)
				{
					var text4 = Program.ValidateFilePath(filePath2, ".aei", false, true);
					if (text4 == null)
					{
						return ConsoleResult.ErrorInvalidPath;
					}
					scene.GetResource().Path = text4;
				}
				KeyValuePair<int, string>[] array;
				var consoleResult = ParseParams(args, actionArgCount, scene, flag2, flag2, out array);
				if (consoleResult != ConsoleResult.Ok)
				{
					return consoleResult;
				}
				if (flag2)
				{
					var text5 = Program.GetValidatedPath(filePath2, true);
					if (text5 == null)
					{
						return ConsoleResult.ErrorInvalidPath;
					}
					if (array != null)
					{
						scene.ExportTextures(text5, array);
					}
					else
					{
						scene.ExportTextures(text5);
					}
				}
				else if (actionId == 5)
				{
					var text6 = Program.GetValidatedPath(filePath2);
					if (text6 == null)
					{
						return ConsoleResult.ErrorInvalidPath;
					}
					if (array != null)
					{
						scene.ImportTextures(text6, array);
					}
					else
					{
						scene.ImportTextures(text6);
					}
				}
			}
			if (!flag2 && !flag)
			{
				Program.DoAsync(scene.GetResource().Write, "Writing data");
			}
			return ConsoleResult.Ok;
		}

		public static ConsoleResult DoWork(string[] args)
		{
			var text = args[0];
			var num = Array.FindIndex(_actionNames, text.Equals);
			if (num < 0)
			{
				ShowError("Unknown operation " + text);
				return ConsoleResult.ErrorAction;
			}
			var num2 = args.Length - 1;
			var num3 = _actionArgCounts[num];
			if (num2 >= num3)
			{
				var flag = false;
				string text2;
				var consoleResult = PreParseParams(args, num3, out flag, out text2);
				if (flag && text2 != null)
				{
					text2 = Program.GetValidatedPath(text2);
					if (text2 == null)
					{
						return ConsoleResult.ErrorInvalidPath;
					}
				}
				if (consoleResult == ConsoleResult.Ok)
				{
					if (num != 0)
					{
						using (var aescene = new AeScene())
						{
							aescene.WorkStart();
							if (flag)
							{
								var text3 = Program.GetValidatedPath(args[1]);
								if (text3 == null)
								{
									return ConsoleResult.ErrorInvalidPath;
								}
								string[] array = null;
								var num4 = 0;
								var flag2 = text2 != null;
								if (flag2)
								{
									array = Directory.GetFiles(text2, "*.png");
									num4 = array.Length;
								}
								switch (num)
								{
								case 1:
								{
									var text4 = Program.GetValidatedPath(args[2], true);
									if (text4 == null)
									{
										return ConsoleResult.ErrorInvalidPath;
									}
									var files = Directory.GetFiles(text3, "*.png");
									for (var i = 0; i < files.Length; i++)
									{
										var text5 = files[i];
										var texturePath = !flag2 || i >= num4 ? null : array[i];
										consoleResult = DoAction(args, num3, num, text5, Path.Combine(text4, Path.GetFileNameWithoutExtension(text5) + ".aei"), texturePath, aescene);
										if (consoleResult != ConsoleResult.Ok)
										{
											return consoleResult;
										}
									}
									goto IL_387;
								}
								case 2:
								{
									var files2 = Directory.GetFiles(text3, "*.aei");
									for (var j = 0; j < files2.Length; j++)
									{
										var filePath = files2[j];
										var texturePath2 = !flag2 || j >= num4 ? null : array[j];
										consoleResult = DoAction(args, num3, num, filePath, null, texturePath2, aescene);
										if (consoleResult != ConsoleResult.Ok)
										{
											return consoleResult;
										}
									}
									goto IL_387;
								}
								case 3:
								{
									var text6 = Program.GetValidatedPath(args[2], true);
									if (text6 == null)
									{
										return ConsoleResult.ErrorInvalidPath;
									}
									var files3 = Directory.GetFiles(text3, "*.aei");
									for (var k = 0; k < files3.Length; k++)
									{
										var text7 = files3[k];
										var texturePath3 = !flag2 || k >= num4 ? null : array[k];
										consoleResult = DoAction(args, num3, num, text7, Path.Combine(text6, Path.GetFileName(text7)), texturePath3, aescene);
										if (consoleResult != ConsoleResult.Ok)
										{
											return consoleResult;
										}
									}
									goto IL_387;
								}
								case 4:
								{
									var text8 = Program.GetValidatedPath(args[2], true);
									if (text8 == null)
									{
										return ConsoleResult.ErrorInvalidPath;
									}
									var files4 = Directory.GetFiles(text3, "*.aei");
									for (var l = 0; l < files4.Length; l++)
									{
										var text9 = files4[l];
										var texturePath4 = !flag2 || l >= num4 ? null : array[l];
										consoleResult = DoAction(args, num3, num, text9, Path.Combine(text8, Path.GetFileNameWithoutExtension(text9) + ".png"), texturePath4, aescene);
										if (consoleResult != ConsoleResult.Ok)
										{
											return consoleResult;
										}
									}
									goto IL_387;
								}
								case 6:
								{
									var filePath2 = args[2];
									var files5 = Directory.GetFiles(text3, "*.aei");
									for (var m = 0; m < files5.Length; m++)
									{
										var filePath3 = files5[m];
										var texturePath5 = !flag2 || m >= num4 ? null : array[m];
										consoleResult = DoAction(args, num3, num, filePath3, filePath2, texturePath5, aescene);
										if (consoleResult != ConsoleResult.Ok)
										{
											return consoleResult;
										}
									}
									goto IL_387;
								}
								}
								ShowError("Unsupported batch action: " + text);
								return ConsoleResult.ErrorAction;
							}

                            consoleResult = DoAction(args, num3, num, args[1], args[2], text2, aescene);
                            IL_387:
							aescene.WorkEnd();
						}
						return consoleResult;
					}
					UiManager.ShowMessage("\r\nCommand format: AEIEditor Action [Parameters separated by spaces]\r\n\r\nAction:\r\n    Help                        -   show this text,\r\n                                        all parameters are ignored\r\n    Create FilePNG FileAEI      -   create AEI file from PNG texture,\r\n                                        the following parameters are ignored::\r\n                                            TexturePath,\r\n                                            TextureMap\r\n    Change FileAEI              -   save AEI file\r\n                                        parameter TextureMap are ignored\r\n    Save FileAEI NewFileAEI   -   save new AEI file\r\n                                        parameter TextureMap are ignored\r\n    Export FileAEI FilePNG      -   save PNG texture from AEI file,\r\n                                        all parameters are ignored\r\n    ImportTextures FileAEI Path -   import texture fragments from PNG\r\n                                        files at the specified absolute path,\r\n                                        all in alphabetical order,\r\n                                        or bound by TextureMap\r\n    ExportTextures FileAEI Путь -   export texture fragments to PNG\r\n                                        files at the specified absolute path,\r\n                                        names are set according to the scheme:\r\n                                            TextureNumber.AEIFileName\r\n                                        or bound by TextureMap,\r\n                                        all other parameters are ignored\r\n\r\nParameters and their default values:\r\n\r\n    Silent=false    -   true if the program should not require\r\n                            user actions and display an interface\r\n    TexturePath=    -   Texture file to replace\r\n    Format=RAW      -   Compression format, options:\r\n                                RAW            -   None\r\n                                RAW_UI         -   None, interface\r\n                                RAW_CubeMap_PC -   None, CubeMap(PC)\r\n                                RAW_CubeMap    -   None, CubeMap\r\n                                DXT1\r\n                                DXT3\r\n                                DXT5\r\n                                ETC1           -   ETC1 no transparency\r\n\t                            PVRTCI2A       -   PVRTC1 2bpp\r\n                                PVRTCI4A       -   PVRTC1 4bpp\r\n                                ATC            -   ATC 4bpp\r\n    MipMapped=false -   Use mip-mapping\r\n    Quality=3       -   Compressed texture quality from 0 to 3\r\n    TextureMap=     -   Binding a file name, separated by commas\r\n                                to texture numbers, through a colon, for example:\r\n                                texA.png:1,texB.png:2,texC.png:3\r\n    BatchMode=false -   Batch mode.\r\n                            Instead of file names, you need to specify folders\r\n                            for working with files in them.\r\n                            Not supported:\r\n                                Operation ImportTextures\r\n                                Parameter TextureMap\r\n\r\nIf you need to specify a path or file name with spaces, use\r\ndouble quotes", "Command line help");
				}
				return consoleResult;
			}
			ShowError("Invalid number of required arguments: " + num2 + "/" + _actionArgCounts);
			return ConsoleResult.ErrorArgCount;
		}

		private const string HelpTxt = "\r\nCommand format: AEIEditor Action [Parameters separated by spaces]\r\n\r\nAction:\r\n    Help                        -   show this text,\r\n                                        all parameters are ignored\r\n    Create FilePNG FileAEI      -   create AEI file from PNG texture,\r\n                                        the following parameters are ignored::\r\n                                            TexturePath,\r\n                                            TextureMap\r\n    Change FileAEI              -   save AEI file\r\n                                        parameter TextureMap are ignored\r\n    Save FileAEI NewFileAEI   -   save new AEI file\r\n                                        parameter TextureMap are ignored\r\n    Export FileAEI FilePNG      -   save PNG texture from AEI file,\r\n                                        all parameters are ignored\r\n    ImportTextures FileAEI Path -   import texture fragments from PNG\r\n                                        files at the specified absolute path,\r\n                                        all in alphabetical order,\r\n                                        or bound by TextureMap\r\n    ExportTextures FileAEI Путь -   export texture fragments to PNG\r\n                                        files at the specified absolute path,\r\n                                        names are set according to the scheme:\r\n                                            TextureNumber.AEIFileName\r\n                                        or bound by TextureMap,\r\n                                        all other parameters are ignored\r\n\r\nParameters and their default values:\r\n\r\n    Silent=false    -   true if the program should not require\r\n                            user actions and display an interface\r\n    TexturePath=    -   Texture file to replace\r\n    Format=RAW      -   Compression format, options:\r\n                                RAW            -   None\r\n                                RAW_UI         -   None, interface\r\n                                RAW_CubeMap_PC -   None, CubeMap(PC)\r\n                                RAW_CubeMap    -   None, CubeMap\r\n                                DXT1\r\n                                DXT3\r\n                                DXT5\r\n                                ETC1           -   ETC1 no transparency\r\n\t                            PVRTCI2A       -   PVRTC1 2bpp\r\n                                PVRTCI4A       -   PVRTC1 4bpp\r\n                                ATC            -   ATC 4bpp\r\n    MipMapped=false -   Use mip-mapping\r\n    Quality=3       -   Compressed texture quality from 0 to 3\r\n    TextureMap=     -   Binding a file name, separated by commas\r\n                                to texture numbers, through a colon, for example:\r\n                                texA.png:1,texB.png:2,texC.png:3\r\n    BatchMode=false -   Batch mode.\r\n                            Instead of file names, you need to specify folders\r\n                            for working with files in them.\r\n                            Not supported:\r\n                                Operation ImportTextures\r\n                                Parameter TextureMap\r\n\r\nIf you need to specify a path or file name with spaces, use\r\ndouble quotes";

		private static string[] _actionNames = {
			"Help",
			"Create",
			"Change",
			"Save",
			"Export",
			"ImportTextures",
			"ExportTextures"
		};

		private static string[] _paramNames = {
			"Silent",
			"TexturePath",
			"Format",
			"MipMapped",
			"Quality",
			"TextureMap",
			"BatchMode"
		};

		private static int[] _actionArgCounts = {
			0,
			2,
			1,
			2,
			2,
			2,
			2
		};
	}
}
