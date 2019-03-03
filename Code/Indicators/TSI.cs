
using System;
using System.ComponentModel;
using System.Windows.Media;
using Alveo.Interfaces.UserCode;
using Alveo.UserCode;
using Alveo.Common;
using Alveo.Common.Classes;
using System.Collections.Generic;

namespace Alveo.UserCode
{
    [Serializable]
    public class TSI : IndicatorBase
    {
    	
    	Array<double> TSI_Buffer = new Array<double>();
        Array<double> SignalBuffer = new Array<double>();
        Array<double> MTM_Buffer = new Array<double>();
        Array<double> EMA_MTM_Buffer = new Array<double>();
        Array<double> EMA2_MTM_Buffer = new Array<double>();
        Array<double> ABSMTM_Buffer = new Array<double>();
        Array<double> EMA_ABSMTM_Buffer = new Array<double>();
        Array<double> EMA2_ABSMTM_Buffer = new Array<double>();
        Array<double> lvl1 = new Array<double>();
        Array<double> lvl2 = new Array<double>();
        Array<double> lvl3 = new Array<double>();

        #region Properties

        [Category("Settings")]
        public int First_R{ get; set; }

        [Category("Settings")]
        public int Second_S{ get; set; }

        [Category("Settings")]
        public int SignalPeriod{ get; set; }

        #endregion

        //+------------------------------------------------------------------+
        //| Custom indicator default constructor. DO NOT REMOVE              |
        //+------------------------------------------------------------------+
        public TSI()
        {
            copyright = "";
            link = "";
            indicator_separate_window = true;
            indicator_buffers = 5;
            indicator_color1 = DodgerBlue;
            indicator_color2 = Magenta;
            indicator_maximum = 100;
            indicator_minimum = -100;
            indicator_level1 = 0;
            indicator_level2 = 25;
            indicator_level3 = -25;

            First_R = 25;
            Second_S = 13;
            SignalPeriod = 13;
        }
        
        protected override int Init()
        {            
            IndicatorBuffers(11);
            
            SetIndexStyle(0, DRAW_LINE);
            SetIndexBuffer(0, TSI_Buffer);
            SetIndexLabel(0, "TSI" + "(" + First_R + "," + Second_S + "," + SignalPeriod + ")");
            
            SetIndexStyle(1, DRAW_LINE);
            SetIndexBuffer(1, SignalBuffer);
            SetIndexLabel(1, "Signal");
            
            SetIndexStyle(2, DRAW_LINE, (int) Levels.Style, Levels.Width, Levels.Color);
            SetIndexBuffer(2, lvl1);
            SetIndexLabel(2, "Level 1");
            
            SetIndexStyle(3, DRAW_LINE, (int) Levels.Style, Levels.Width, Levels.Color);
            SetIndexBuffer(3, lvl2);
            SetIndexLabel(3, "Level 2");
            
            SetIndexStyle(4, DRAW_LINE, (int) Levels.Style, Levels.Width, Levels.Color);
            SetIndexBuffer(4, lvl3);
            SetIndexLabel(4, "Level 3");
            
            SetIndexBuffer(5, ABSMTM_Buffer);
            SetIndexBuffer(6, EMA_ABSMTM_Buffer);
            SetIndexBuffer(7, EMA2_ABSMTM_Buffer);
            SetIndexBuffer(8, MTM_Buffer);
            SetIndexBuffer(9, EMA_MTM_Buffer);
            SetIndexBuffer(10, EMA2_MTM_Buffer);  
            
            IndicatorShortName("TSI" + "(" + First_R + "," + Second_S + "," + SignalPeriod + ")");

            return(0);
        }

        protected override int Start()
        {
        	
        	int counted_bars = IndicatorCounted();
            int limit, i;

            limit = Bars - counted_bars - 1;
            
            for(i = Bars - 1; i >= 0; i--)
            {
                MTM_Buffer[i] = (Close[i] - Close[i+1]);
                ABSMTM_Buffer[i] = Math.Abs(MTM_Buffer[i]);
            }

            for(i = Bars - 1; i >= 0; i--)
            {
                EMA_MTM_Buffer[i] = iMAOnArray(MTM_Buffer, 0, First_R, 0, MODE_EMA, i);
                EMA_ABSMTM_Buffer[i] = iMAOnArray(ABSMTM_Buffer, 0, First_R, 0, MODE_EMA, i);
            }

            for(i = Bars - 1; i >= 0; i--)
            {
                EMA2_MTM_Buffer[i] = iMAOnArray(EMA_MTM_Buffer, 0, Second_S, 0, MODE_EMA, i);
                EMA2_ABSMTM_Buffer[i] = iMAOnArray(EMA_ABSMTM_Buffer, 0, Second_S, 0, MODE_EMA, i);
            }

            for(i = limit; i >= 0; i--)
            {
                TSI_Buffer[i] = 100.0*EMA2_MTM_Buffer[i] / EMA2_ABSMTM_Buffer[i];
            }
            
            for(i = limit; i >= 0; i--)
            {
                SignalBuffer[i] = iMAOnArray(TSI_Buffer, 0, SignalPeriod, 0, MODE_EMA, i);
            }
            
            for (i = 0; i < lvl1.Count; i++)
            {
                lvl1[i] = Levels.Values[0].Value;
            }
            
            for (i = 0; i < lvl2.Count; i++)
            {
                lvl2[i] = Levels.Values[1].Value;
            }
            
            for (i = 0; i < lvl3.Count; i++)
            {
                lvl3[i] = Levels.Values[2].Value;
            }
            
            return(0);
        }

    }
}
