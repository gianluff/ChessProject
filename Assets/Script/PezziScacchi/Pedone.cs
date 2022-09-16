using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedone : Pezzo
{
    public override List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool skill)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direzione = (schieramento == 0) ? 1 : -1;
        if(scacchiera[XCorrente, YCorrente + direzione] == null)
        {
            r.Add(new Vector2Int(XCorrente, YCorrente + direzione));
        }

        //Movimento di due caselle se il pedone è in posizione iniziale
        if (scacchiera[XCorrente, YCorrente + direzione] == null)
        {
            if (schieramento == 0 && YCorrente == 1 && scacchiera[XCorrente, YCorrente + (direzione * 2)] == null)
                r.Add(new Vector2Int(XCorrente, YCorrente + (direzione * 2)));
            if (schieramento == 1 && YCorrente == 6 && scacchiera[XCorrente, YCorrente + (direzione * 2)] == null)
                r.Add(new Vector2Int(XCorrente, YCorrente + (direzione * 2)));
        }

        //Mossa in diagonale per mangiare
        if (XCorrente != numeroCaselleX - 1 /*&& XCorrente != 0*/) {
            if (scacchiera[XCorrente+1, YCorrente + direzione] != null && scacchiera[XCorrente + 1, YCorrente + direzione].schieramento != schieramento)
            {
                r.Add(new Vector2Int(XCorrente + 1, YCorrente + direzione));
            }
        }
        if (XCorrente != 0)
        {
            if (scacchiera[XCorrente -1, YCorrente + direzione] != null && scacchiera[XCorrente - 1, YCorrente + direzione].schieramento != schieramento)
            {
                r.Add(new Vector2Int(XCorrente - 1, YCorrente + direzione));
            }
        }
        return r;
    }

    public override MossaSpeciale OttieniMosseSpeciali(ref Pezzo[,] scacchiera, ref List<Vector2Int[]> listaMosse, ref List<Vector2Int> mosseDisponibili)
    {

        int direzione = (schieramento == 0) ? 1 : -1;

        //Promozione
        if ((schieramento == 0 && YCorrente == 6) || (schieramento == 1 && YCorrente == 1))
            return MossaSpeciale.Promozione;

        //En passant
        if (listaMosse.Count > 0)
        {
            Vector2Int[] ultimaMossa = listaMosse[listaMosse.Count - 1];
            //questa mossa speciale può avvenire solo dopo il movimento di un pedone avversario
            if (scacchiera[ultimaMossa[1].x, ultimaMossa[1].y].tipo == TipoPezzo.Pedone)
            {
                if (Mathf.Abs(ultimaMossa[0].y - ultimaMossa[1].y) == 2) //verificare se è stato uno spostamento di due caselle
                {
                    if (scacchiera[ultimaMossa[1].x, ultimaMossa[1].y].schieramento != schieramento)
                    {
                        if(ultimaMossa[1].y == YCorrente) //Se i due pezzi sono alla stessa altezza sulla scacchiera
                        {
                            if (ultimaMossa[1].x == XCorrente - 1) // se i due pezzi sono accanto
                            {
                                mosseDisponibili.Add(new Vector2Int(XCorrente - 1, YCorrente + direzione));
                                return MossaSpeciale.EnPassant;
                            }
                            if (ultimaMossa[1].x == XCorrente + 1)
                            {
                                mosseDisponibili.Add(new Vector2Int(XCorrente + 1, YCorrente + direzione));
                                return MossaSpeciale.EnPassant;
                            }
                        }
                    }
                }
            }
        }
        return MossaSpeciale.Nessuna;
    }
}
