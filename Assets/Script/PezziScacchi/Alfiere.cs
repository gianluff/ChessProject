using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alfiere : Pezzo
{
    public override List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool skill)
    {
        List<Vector2Int> r = new List<Vector2Int>();

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
