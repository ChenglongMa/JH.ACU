#ifndef GlobalH
#define GlobalH

#pragma once

#define NOWIN32_LEAN_AND_MEAN
#include <shlobj.hpp>
#include <vcl.h>
#include <IniFiles.hpp>
#include <stdio.h>

#define MES_ADDRESS 		0x0602      // Flash起始地址
#define MES_BYTES 			134			// 读取的字节数

// 板卡和继电器
#define MES_SUBBOARD_NUM  		6		// 子板数
#define MES_RELAYGROUP      	9 		// 继电器组数
#define MES_RELAYNUM    		8		// 每组继电器个数

// DMM测量电压、电流、电阻
#define MES_COEFFICIENT 		0.618	// 二分法
#define MES_MAX_WAITTIME		15000 	// ms

// PcPower下运用
#define MES_BUFFER_LENGTH 		80

// FC测试限制条件
#define MES_RECURSIVE_LOW       0.01        
#define MES_RECURSIVE_HIGH      0.001
#define MES_SPEC_BIG            3000.0

///////////////////////////////////////////////////////////////////////////////
// 测试中的重复次数
extern unsigned int 	g_iItemRepeat;		// 单个测试步骤失败时重复次数，其继电器和供电检查已完成，需要硬件重启
extern unsigned int		g_iFindRepeat;		// Kline通信失败时的重复次数，需要重启公共继电器、当前继电器和电源供应检查并供应

///////////////////////////////////////////////////////////////////////////////
// 测试信息提示 on testing / Charmber Process
extern int  	g_iViewChamber; 	// Charmber Process
extern int  	g_iViewTest; 		// on testing
extern bool 	g_bViewChamber;		// Chamber Process
extern bool 	g_bViewTest;		// on testing
extern String	g_strCurrSet;		// current setting

///////////////////////////////////////////////////////////////////////////////
// 温度OnTimer 显示和写入TempSheet
extern bool 	g_bChamberIdle;	   	// 温箱是否空闲
extern bool 	g_bTempReal;		// 是否立取温度
extern int  	g_iTempCount;		// 每10S获取实际温度
extern int  	g_iTempInterval;	// 控制写到文件Interval
extern int		g_iTempRow;			// 写入TempSheet的总行数

///////////////////////////////////////////////////////////////////////////////
// 电压OnTimer 显示和写入Volt.dat
extern int  	g_iVoltCount;		// 每10S获取实际电压
extern bool 	g_bVoltReal;		// 是否立取电压

///////////////////////////////////////////////////////////////////////////////
// 状态条及相关信息显示控制
extern bool 		g_bTimeStart;	// on testing 是否真正开始
extern unsigned int g_iSecond;		// 测试累积时间
extern String 	g_strMainTip;		// 提示信息：大步骤
extern String 	g_strSubTip;		// 提示信息：小流程

///////////////////////////////////////////////////////////////////////////////
// 曲线：温度曲线
extern bool 	g_bTempRenew;		// 重新设置温度
extern float 	g_fTimeTotal;		// 每次重新设置温度估计所需时间 minute
extern float 	g_fTimeRest;		// 每次重新设置温度余下时间
extern float 	g_fTempInit;	 	// 每次重新设置温箱温度时的初始温度
extern float 	g_fTempCurveX;		// 实际温度曲线横坐标 m_pTempCurveReal
extern float	g_fTempCurveY;		// 当前温箱的温度，实际温度曲线纵坐标
///////////////////////////////////////////////////////////////////////////////
// 曲线：测试项曲线
extern int		g_iCurveMode;		// 0-电阻模式 1-HoldTime模式 2-Crash模式
extern double 	g_dCurveCurrX;   	// 测试项曲线显示X
extern float 	g_fCurveCurrY;		// 测试项曲线显示Y
extern double 	g_dCurveLength;		// 测试项曲线显示长度，	与上面对应0-19.0 1-999.0 2-999.0
extern double 	g_daCurveY[1000];   // 在1和2时采样的值集合
extern String	g_strCurveUnit;		// 曲线显示左边单位,	与上面对应0-ohm 1-v 2-v

///////////////////////////////////////////////////////////////////////////////
// 进度信息控制
extern bool 	g_bProPos;			// 是否单步
extern int 		g_iProPos;			// 不是单步时前进到多少

///////////////////////////////////////////////////////////////////////////////
// 测试项
// 温度3、电压3、名称-模型名-评论3、ACU6、时间间隔1、时间延迟3；共19项
#define MES_ITEM_REAL			74		// 进行测试的所有项
#define MES_ITEM_RET			63		// 需要检测错误码
typedef struct Ex_STestInfo
{
        String m_strTemp[3];			// 三种电流值 必须
        String m_strVolt[3];            // 三种电压值 必须
        String m_strInterval;           // 时间间隔（second）必须
        String m_strDelay[3];           // 时间延迟LT、NT、HT（minute）必须
        String m_strInfo[3];            // 测试名称-模型名-评论
        String m_strEcuId[6];           // ECU的ID号
        int m_iTVCond[9]; 	   			// 测试条件：温度电压的九种组合情况
        int m_iItem[MES_ITEM_REAL];		// 测试步骤，每一个包含板卡信息
        int m_iSelACU;    				// 所有被选ACU子板号汇总
        int m_iDaqType;   				// DAQ输出类型 	0 advanced   	1 conventional
        int m_iChamType;  				// 温箱标识    	0 不启动温箱 	1 启动温箱
        int m_iTempSeq[3];				// 温度操作顺序 0 低温 			1 中温 			2 高温
        float m_fPrecision[4];			// 对应test.ini文件中的测试精度
        float m_fAmendRes[8][4];    	// 接触线等电阻，进行修正处理
}STestInfo;
extern STestInfo g_STestInfo;	 	// 测试中当前信息汇总
extern int g_iTempPhase;			// 设置温度阶段 based 0  0-LT 1-NT 2-HT
extern int g_iVoltPhase;			// 设置电压阶段 based 0  0-LV 1-NT 2-HV
extern float g_fTempTarget;			// 当前目标温度，和上面对应
extern float g_fVoltTarget;			// 当前目标电压，和上面对应
extern float g_fVoltInit;	 		// 每次重新设置电源电压时的初始电压
extern float g_fVoltCurr;    		// 实时电源电压
extern int 	g_iCRC;					// 在某一个TempVolt下的某一个板卡下的FM操作次数
extern int 	g_iTVCond;			   	// 正在测试的TV条件，由上面两个组成	based 0
extern int  g_iCurrBoard;			// 正在测试的ECU所连的子板 based 0
extern int  g_iCurrItem;			// 正在测试的当前Item步骤 based 0
extern bool	g_bOpState;				// 操作返回状态
extern int  g_iOpMode;              // 0-自动、1-手工操作模式
extern String	g_strMark;      	// 测试提示信息
extern String 	g_strItemValue;		// 测试项完成后测量的值
void 	SetGlobalStateForNewTest();	// 更新全局变量为了新的测试



///////////////////////////////////////////////////////////////////////////////
// 综合
extern String 	g_strAppPath;		// 程序路径
void 	GetAppPath(void);			// 获取程序运行路径
void 	G_SDelay(DWORD dwMilliSecond);	// 单步等待
void 	G_MDelay(DWORD dwMilliSecond);	// 异步等待
String 	DToH2(int iInput);			// 把1-15数字编程16进制
String 	DToH1(int iInput);			// 编程0xFF模式，只显示16进制后2位
String 	IntToStrAdjust(int iInput);	// 把1-9进行2位输出，大于9直接输出
String 	IntToStrTime(int iSecond);	// 时：分：秒显示
bool 	GetStrType(String strTemp); // 判断是否是纯数字字符串，是返回真，否则返回假


///////////////////////////////////////////////////////////////////////////////
// 设备：串口-温箱、Kline、D2KDASK-采集卡、
//		 GPIB-接口、电源（状态、设置）、DMM、RES1、RES2、
//		 状态-供电状态
extern bool g_bAllDevice;	// 硬件设备是否良好
bool 	OpenDevice(); 		// 打开所有设备
bool    CloseDevice(); 		// 关闭所遇设备


///////////////////////////////////////////////////////////////////////////////
// Kline串口数据
extern FILE     *g_pFileComKline;
extern FILE	    *g_pFileErrReal;
extern FILE		*g_pFileVolt;
bool    OpenLogFile(int iFileType, String strPath);	// 打开Log文件
bool    WriteLogFile(FILE *pFile, String str);		// 写入数据
bool    CloseLogFile(FILE *pFile);                 	// 关闭Log文件


///////////////////////////////////////////////////////////////////////////////
// 错误处理
// 规范和错误
#define MES_ERRINFO_LENGTH		65		// 错误代码的最大长度 最大长度59 + 2 就可以
#define MES_SPEC_LENGTH 		85		// 测试规范的最大长度 最大长度79 + 2 就可以
// 测试Fault_Code.txt中给出的最大错误数
#define MES_ERRORS_NUM 			220		// 总的错误代码数量
// lISTBOX
#define MES_LISTLINE_HEIGHT 	14    	// 行高
#define MES_LISTLINE_DISTANCE 	2      	// 行间距
extern int	g_iCurrFaultCode;			// 当前的错误码
extern char	g_caSpec[MES_ITEM_REAL][MES_SPEC_LENGTH];			// 读取后保持不变 SPEC.TXT
extern char g_caErrInfo[MES_ERRORS_NUM][MES_ERRINFO_LENGTH];	// 读取后保持不变 FAULTCODE.TXT
void 	SetSPECInfo();	// 读取SPEC.TXT，配置g_caSpec
String 	GetSPECInfo(int iFullItem, int iType);	// iFullItem based 0， iType based 0
void 	ReadFaultInfo();// 读取FAULTCODE.TXT，配置g_caSpec
bool	GetFaultInfo(int iFaultCode, String & strCode, String & strRemark);

#endif
