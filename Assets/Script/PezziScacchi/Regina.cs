using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regina : Pezzo
{
    public override List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool skill)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Giù
        for (int j = YCorrente - 1; j >= 0; j--)
        {
            if (scacchiera[XCorrente, j] == null)
                r.Add(new Vector2Int(XCorrente, j));

            if (scacchiera[XCorrente, j] != null)
            {
                if (scacchiera[XCorrente, j].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente, j));
                break;
            }

        }

        //Su
        for (int j = YCorrente + 1; j < numeroCaselleY; j++)
        {
            if (scacchiera[XCorrente, j] == null)
                r.Add(new Vector2Int(XCorrente, j));

            if (scacchiera[XCorrente, j] != null)
            {
                if (scacchiera[XCorrente, j].schieramento != schieramento)
                    r.Add(new Vector2Int(XCorrente, j));
                break;
            }

        }

        //Destra
        for (int j = XCorrente + 1; j < numeroCaselleX; j++)
        {
            if (scacchiera[j, YCorrente] == null)
                r.Add(new Vector2Int(j, YCorrente));

            if (scacchiera[j, YCorrente] != null)
            {
                if (scacchiera[j, YCorrente].schieramento != schieramento)
                    r.Add(new Vector2Int(j, YCorrente));
                break;
            }

        }

        //Sinistra
        for (int j = XCorrente - 1; j >= 0; j--)
        {
            if (scacchiera[j, YCorrente] == null)
                r.Add(new Vector2Int(j, YCorrente));

            if (scacchiera[j, YCorrente] != null)
            {
                if (scacchiera[j, YCorrente].schieramento != schieramento)
                    r.Add(new Vector2Int(j, YCorrente));
                break;
            }

        }

        //Giù sinistra
        int i = XCorrente - 1;
        for (int j = YCorrente - 1; j >= 0; j--)
        {
            if (i >= 0)
            {
                if (scacchiera[i, j] == null)
                    r.Add(new Vector2Int(i, j));

                if (scacchiera[i, j] != null)
                {
                    if (scacchiera[i, j].schieramento != schieramento)
                        r.Add(new Vector2Int(i, j));
                    break;
                }
            }
            else
                break;
            i = i - 1;
        }

        //Giù destra
        i = XCorrente + 1;
        for (int j = YCorrente - 1; j >= 0; j--)
        {
            if (i < numeroCaselleX)
            {
                if (scacchiera[i, j] == null)
                    r.Add(new Vector2Int(i, j));

                if (scacchiera[i, j] != null)
                {
                    if (scacchiera[i, j].schieramento != schieramento)
                        r.Add(new Vector2Int(i, j));
                    break;
                }
            }
            else
                break;
            i = i + 1;
        }

        //Su sinistra
        i = XCorrente - 1;
        for (int j = YCorrente + 1; j < numeroCaselleY; j++)
        {
            if (i >= 0)
            {
                if (scacchiera[i, j] == null)
                    r.Add(new Vector2Int(i, j));

                if (scacchiera[i, j] != null)
                {
                    if (scacchiera[i, j].schieramento != schieramento)
                        r.Add(new Vector2Int(i, j));
                    break;
                }
            }
            else
                break;
            i = i - 1;
        }

        //Su destra
        i = XCorrente + 1;
        for (int j = YCorrente + 1; j < numeroCaselleY; j++)
        {
            if (i < numeroCaselleX)
            {
                if (scacchiera[i, j] == null)
                    r.Add(new Vector2Int(i, j));

                if (scacchiera[i, j] != null)
                {
                    if (scacchiera[i, j].schieramento != schieramento)
                        r.Add(new Vector2Int(i, j));
                    break;
                }
            }
            else
                break;
            i = i + 1;
        }

        return r;
    }
}
