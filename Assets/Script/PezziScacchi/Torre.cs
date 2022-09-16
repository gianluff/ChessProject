using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Torre : Pezzo
{
    public override List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool berserk)
    {
        
        List<Vector2Int> r = new List<Vector2Int>();
        //serve una serie di for per valutare tutte le caselle libere in tutte le direzioni
        if (!berserk)
        {
            //Giù
            for (int i = YCorrente - 1; i >= 0; i--)
            {
                if (scacchiera[XCorrente, i] == null)
                    r.Add(new Vector2Int(XCorrente, i));

                if (scacchiera[XCorrente, i] != null)
                {
                    if (scacchiera[XCorrente, i].schieramento != schieramento)
                        r.Add(new Vector2Int(XCorrente, i));
                    break;
                }

            }
            //Su

            for (int i = YCorrente + 1; i < numeroCaselleY; i++)
            {
                if (scacchiera[XCorrente, i] == null)
                    r.Add(new Vector2Int(XCorrente, i));

                if (scacchiera[XCorrente, i] != null)
                {
                    if (scacchiera[XCorrente, i].schieramento != schieramento)
                        r.Add(new Vector2Int(XCorrente, i));
                    break;
                }

            }

            //Destra
            for (int i = XCorrente + 1; i < numeroCaselleX; i++)
            {
                if (scacchiera[i, YCorrente] == null)
                    r.Add(new Vector2Int(i, YCorrente));

                if (scacchiera[i, YCorrente] != null)
                {
                    if (scacchiera[i, YCorrente].schieramento != schieramento)
                        r.Add(new Vector2Int(i, YCorrente));
                    break;
                }

            }

            //Sinistra
            for (int i = XCorrente - 1; i >= 0; i--)
            {
                if (scacchiera[i, YCorrente] == null)
                    r.Add(new Vector2Int(i, YCorrente));

                if (scacchiera[i, YCorrente] != null)
                {
                    if (scacchiera[i, YCorrente].schieramento != schieramento)
                        r.Add(new Vector2Int(i, YCorrente));
                    break;
                }

            }
        }
        //MOSSA SPECIALE BERSERK 
        //controllo di non trovare un re (nemico o alleato sulla traiettoria)
        if (berserk)
        {
            bool reTrovato = false;
            if (XCorrente == 0 && YCorrente == 0)
            {
                for (int i = 1; i <= 7; i++)
                {
                    if (scacchiera[i, 0] != null)
                    {
                        if (scacchiera[i, 0].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(7, 0));
                reTrovato = false;
                for (int j = 1; j <= 7; j++)
                {
                    if (scacchiera[0, j] != null)
                    {
                        if (scacchiera[0, j].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(0, 7));
            }
            else if (XCorrente == 0 && YCorrente == 7)
            {
                for (int i = 1; i <= 7; i++)
                {
                    if (scacchiera[i, 7] != null)
                    {
                        if (scacchiera[i, 7].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(7, 7));
                reTrovato = false;
                for (int j = 6; j >= 0; j--)
                {
                    if (scacchiera[0, j] != null)
                    {
                        if (scacchiera[0, j].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(0, 0));
            }
            else if (XCorrente == 7 && YCorrente == 0)
            {
                for (int i = 6; i >= 0; i--)
                {
                    if (scacchiera[i, 0] != null)
                    {

                        if (scacchiera[i, 0].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(0, 0));
                reTrovato = false;
                for (int j = 0; j <= 7; j++)
                {
                    if (scacchiera[7, j] != null)
                    {
                        if (scacchiera[7, j].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(7, 7));
            }
            else if (XCorrente == 7 && YCorrente == 7)
            {
                for (int i = 6; i >= 0; i--)
                {
                    if (scacchiera[i, 7] != null)
                    {
                        if (scacchiera[i, 7].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(0, 7));
                reTrovato = false;
                for (int j = 6; j >= 0; j--)
                {
                    if (scacchiera[7, j] != null)
                    {
                        if (scacchiera[7, j].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato)
                    r.Add(new Vector2Int(7, 0));
            }
            else
            {
                reTrovato = false;
                for (int i = XCorrente - 1; i >= 0; i--)
                {
                    if (scacchiera[i, YCorrente] != null)
                    {
                        if (scacchiera[i, YCorrente].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato && XCorrente != 0)
                    r.Add(new Vector2Int(0, YCorrente));

                reTrovato = false;
                for (int i = XCorrente + 1; i <= 7; i++)
                {
                    if (scacchiera[i, YCorrente] != null)
                    {
                        if (scacchiera[i, YCorrente].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato && XCorrente != 7)
                    r.Add(new Vector2Int(7, YCorrente));

                reTrovato = false;
                for (int j = YCorrente - 1; j >= 0; j--)
                {
                    if (scacchiera[XCorrente, j] != null)
                    {
                        if (scacchiera[XCorrente, j].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato && YCorrente != 0)
                    r.Add(new Vector2Int(XCorrente, 0));

                reTrovato = false;
                for (int j = YCorrente + 1; j <= 7; j++)
                {
                    if (scacchiera[XCorrente, j] != null)
                    {
                        if (scacchiera[XCorrente, j].tipo == TipoPezzo.Re)
                            reTrovato = true;
                    }
                }
                if (!reTrovato && YCorrente != 7)
                    r.Add(new Vector2Int(XCorrente, 7));
            }
        }
        return r;
    }
}
