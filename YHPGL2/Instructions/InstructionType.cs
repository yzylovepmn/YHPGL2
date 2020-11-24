using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum InstructionType
    {
        Unknow,

        // The Configuration and Status Group
        CO,
        DF,
        IN,
        IP,
        IR,
        IW,
        PG,
        RO,
        RP,
        SC,

        // The Vector Group
        AA,
        AR,
        AT,
        CI,
        PA,
        PD,
        PE,
        PR,
        PU,
        RT,

        // The Polygon Group
        EA,
        EP,
        ER,
        EW,
        FP,
        PM,
        RA,
        RR,
        WG,

        // The Line and Fill Attributes Group
        AC,
        FT,
        LA,
        LT,
        PW,
        RF,
        SM,
        SP,
        UL,
        WU,

        // The Character Group
        AD,
        CF,
        CP,
        DI,
        DR,
        DT,
        DV,
        ES,
        LB,
        LO,
        SA,
        SD,
        SI,
        SL,
        SR,
        SS,
        TD,
    }
}