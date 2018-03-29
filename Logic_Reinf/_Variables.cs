using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Reinf
{
    public static class _V_
    {
        //taken constants
        public static double Z_DRAWING_SCALE = 40;        

        //given constants
        public static int X_ELEMENT_WIDTH = 200;

        public static int X_REINFORCEMENT_MAIN_DIAMETER = 12;
        public static int X_REINFORCEMENT_MAIN_ANCHOR_LENGTH = 500;        

        public static double X_FIRST_PASS_CONSTRAINT = 1.2; // A + 2B vs D

        public static int X_REINFORCEMENT_DIAGONAL_DIAMETER = 8;
        public static int X_REINFORCEMENT_DIAGONAL_ANCHOR_LENGTH = 400;

        public static int X_REINFORCEMENT_STIRRUP_DIAMETER = 8;
        public static int X_REINFORCEMENT_STIRRUP_SPACING = 150;
        public static int X_REINFORCEMENT_STIRRUP_CONSTRAINT = 4; //THIS

        public static int X_REINFORCEMENT_SIDE_D_CREATE = 1;
        public static int X_REINFORCEMENT_SIDE_D_DIAMETER = 8;
        public static int X_REINFORCEMENT_SIDE_D_ANCHOR_LENGTH = 300;
        public static int X_REINFORCEMENT_SIDE_D_SPACING = 400;
        public static int X_REINFORCEMENT_SIDE_D_FIX = -30;

        public static int X_REINFORCEMENT_TOP_D_CREATE = 1;
        public static int X_REINFORCEMENT_TOP_D_DIAMETER = 8;
        public static int X_REINFORCEMENT_TOP_D_ANCHOR_LENGTH = 300;
        public static int X_REINFORCEMENT_TOP_D_SPACING = 400;
        public static int X_REINFORCEMENT_TOP_D_FIX = 0;

        public static string X_REINFORCEMENT_MARK = "B500B";
        public static int X_REINFORCEMENT_NUMBER = 2;

        public static int X_CONCRETE_COVER_1 = 35;


        // calculated constants
        public static double Y_REINFORCEMENT_MAIN_MIN_LENGTH = X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 2 * X_FIRST_PASS_CONSTRAINT; // A + 2B vs D
        public static int Y_REINFORCEMENT_MAIN_RADIUS = 1000;
        public static double X_REINFORCEMENT_MAX_D_LENGTH = 5.5 * X_REINFORCEMENT_MAIN_ANCHOR_LENGTH; // NOT ASKED
        public static double X_REINFORCEMENT_REMOVE_A_LENGTH = 0.9 * Y_REINFORCEMENT_MAIN_MIN_LENGTH; // NOT ASKED
        public static double X_MERGE_EXTEND_SINGLE_DIST = 25; // NOT ASKED
        public static double X_MERGE_EXTEND_DOUBLE_DIST = 50; // NOT ASKED

        public static int Y_STIRRUP_MAX_LENGTH = X_REINFORCEMENT_STIRRUP_CONSTRAINT * X_ELEMENT_WIDTH;
        public static int Y_REINFORCEMENT_STIRRUP_RADIUS = 1000;
        public static int Y_ELEMENT_WIDTH_COVER = X_ELEMENT_WIDTH - X_CONCRETE_COVER_1 * 2;

        public static int Y_CONCRETE_COVER_DELTA = (int)(Math.Ceiling(X_REINFORCEMENT_MAIN_DIAMETER / 5.0 + 0.01) * 5) + 5;
        public static int X_CONCRETE_COVER_2 = X_CONCRETE_COVER_1 + Y_CONCRETE_COVER_DELTA;
        public static int Y_CONCRETE_COVER_3 = X_CONCRETE_COVER_2 + Y_CONCRETE_COVER_DELTA;

        public static double M_TRIM_TOLERANCE = 0.95;
        public static double M_B_BAR_TOLERANCE = 0.02; // B vs C (1 deg == 0.0017)
        public static double M_LINE_SEGMENTATOR_STEP = 50.0;

        public static double X_DENIER_MINIMUM_DELTA = 5.49;
        public static double X_TRIM_MINIMUM_DELTA = 0.05;
        public static double X_TRIM_SIDE_MINIMUM_DELTA = 5.01;
    }
}
