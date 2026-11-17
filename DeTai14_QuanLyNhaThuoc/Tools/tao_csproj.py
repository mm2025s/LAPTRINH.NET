"""Sinh QuanLyNhaThuoc.csproj tu cac tep .cs hien co (chay lai khi them tep moi)."""
import glob
import os

GOC = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
os.chdir(GOC)
BS = chr(92)

dau = """<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)/$(MSBuildToolsVersion)/Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)/$(MSBuildToolsVersion)/Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{F7A8B9C0-D1E2-4F30-8B4C-5D6E7F8A9B14}</ProjectGuid>
    <OutputType>WinExe</OutputType>
    <RootNamespace>QuanLyNhaThuoc</RootNamespace>
    <AssemblyName>QuanLyNhaThuoc</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <Deterministic>true</Deterministic>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin/Debug/</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin/Release/</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
""".replace("/", BS).replace("<" + BS, "</").replace(BS + ">", "/>").replace("http:" + BS + BS + "schemas.microsoft.com" + BS + "developer" + BS + "msbuild" + BS + "2003", "http://schemas.microsoft.com/developer/msbuild/2003")

refs = ["System", "System.Configuration", "System.Core", "System.Data", "System.Data.DataSetExtensions",
        "System.Drawing", "System.Windows.Forms", "System.Xml"]


def win(p):
    return p.replace("/", BS).replace(os.sep, BS)


out = [dau, "  <ItemGroup>\n"]
out += ['    <Reference Include="%s" />\n' % r for r in refs]
out.append("  </ItemGroup>\n  <ItemGroup>\n")
thuong = sorted(glob.glob("DTO/*.cs") + glob.glob("DAL/*.cs") + glob.glob("BUS/*.cs") + glob.glob("Tools/*.cs"))
thuong += ["Program.cs"] + sorted(f for f in glob.glob("GUI/Helpers/*.cs") if not f.replace(os.sep, "/").endswith(("UiControls.cs", "FormBase.cs")))
for f in thuong:
    out.append('    <Compile Include="%s" />\n' % win(f))
out.append('    <Compile Include="%s">\n      <SubType>Component</SubType>\n    </Compile>\n' % win("GUI/Helpers/UiControls.cs"))
for f in ["GUI/Helpers/FormBase.cs"] + sorted(glob.glob("GUI/frm*.cs")):
    out.append('    <Compile Include="%s">\n      <SubType>Form</SubType>\n    </Compile>\n' % win(f))
out.append("  </ItemGroup>\n  <ItemGroup>\n")
for f in ["App.config", "README.txt"]:
    out.append('    <None Include="%s" />\n' % win(f))
# Script SQL chep vao bin/<cau hinh>/SQL de lan chay dau tu tao CSDL.
for f in ["SQL/01_TaoCSDL_QLNhaThuoc.sql", "SQL/02_DuLieuMau.sql"]:
    out.append('    <Content Include="%s">\n      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\n    </Content>\n' % win(f))
out.append('  </ItemGroup>\n  <Import Project="$(MSBuildToolsPath)%sMicrosoft.CSharp.targets" />\n</Project>\n' % BS)

with open("QuanLyNhaThuoc.csproj", "w", encoding="utf-8") as fh:
    fh.write("".join(out))
print("ok")
