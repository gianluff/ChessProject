using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cavallo : Pezzo
{
    public override List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool skill)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        if (((XCorrente+1) < numeroCaselleX) && ((YCorrente + 2) < numeroCaselleY))
        {
            if (scacchiera[XCorrente + 1, YCorrente + 2] == null)
                r.Add(new Vector2Int(XCorrente + 1, YCorrente + 2));

            if (scacchiera[XCorrente + 1, YCorrente + 2] != null)
            {
                if (scacchiera[XCorrente + 1, YCorrente + 2].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente + 1, YCorrente + 2));
            }
        }

        if (((XCorrente + 2) < numeroCaselleX) && ((YCorrente + 1) < numeroCaselleY))
        {
            if (scacchiera[XCorrente + 2, YCorrente + 1] == null)
                r.Add(new Vector2Int(XCorrente + 2, YCorrente + 1));

            if (scacchiera[XCorrente + 2, YCorrente + 1] != null)
            {
                if (scacchiera[XCorrente + 2, YCorrente + 1].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente + 2, YCorrente + 1));
            }
        }

        if (((XCorrente - 1) >= 0) && ((YCorrente - 2) >= 0))
        {
            if (scacchiera[XCorrente - 1, YCorrente - 2] == null)
                r.Add(new Vector2Int(XCorrente - 1, YCorrente - 2));

            if (scacchiera[XCorrente - 1, YCorrente - 2] != null)
            {
                if (scacchiera[XCorrente - 1, YCorrente - 2].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente - 1, YCorrente - 2));
            }
        }

        if (((XCorrente - 2) >= 0) && ((YCorrente - 1) >= 0))
        {
            if (scacchiera[XCorrente - 2, YCorrente - 1] == null)
                r.Add(new Vector2Int(XCorrente - 2, YCorrente - 1));

            if (scacchiera[XCorrente - 2, YCorrente - 1] != null)
            {
                if (scacchiera[XCorrente - 2, YCorrente - 1].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente - 2, YCorrente - 1));
            }
        }

        if (((XCorrente - 1) >= 0) && ((YCorrente + 2) < numeroCaselleY))
        {
            if (scacchiera[XCorrente - 1, YCorrente + 2] == null)
                r.Add(new Vector2Int(XCorrente - 1, YCorrente + 2));

            if (scacchiera[XCorrente - 1, YCorrente + 2] != null)
            {
                if (scacchiera[XCorrente - 1, YCorrente + 2].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente - 1, YCorrente + 2));
            }
        }

        if (((XCorrente - 2) >= 0) && ((YCorrente + 1) < numeroCaselleY))
        {
            if (scacchiera[XCorrente - 2, YCorrente + 1] == null)
                r.Add(new Vector2Int(XCorrente - 2, YCorrente + 1));

            if (scacchiera[XCorrente - 2, YCorrente + 1] != null)
            {
                if (scacchiera[XCorrente - 2, YCorrente + 1].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente - 2, YCorrente + 1));
            }
        }

        if (((XCorrente + 1) < numeroCaselleX) && ((YCorrente - 2) >= 0))
        {
            if (scacchiera[XCorrente + 1, YCorrente - 2] == null)
                r.Add(new Vector2Int(XCorrente + 1, YCorrente - 2));

            if (scacchiera[XCorrente + 1, YCorrente - 2] != null)
            {
                if (scacchiera[XCorrente + 1, YCorrente - 2].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente + 1, YCorrente - 2));
            }
        }

        if (((XCorrente + 2) < numeroCaselleX) && ((YCorrente - 1) >= 0))
        {
            if (scacchiera[XCorrente + 2, YCorrente - 1] == null)
                r.Add(new Vector2Int(XCorrente + 2, YCorrente - 1));

            if (scacchiera[XCorrente + 2, YCorrente - 1] != null)
            {
                if (scacchiera[XCorrente + 2, YCorrente - 1].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente + 2, YCorrente - 1));
            }
        }

        return r;
    }
}
