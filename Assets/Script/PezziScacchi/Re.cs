using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Re : Pezzo
{
    public override List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool skill)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Giù
        if (YCorrente-1 >= 0)
        {
            if ((scacchiera[XCorrente, YCorrente - 1] == null) || (scacchiera[XCorrente, YCorrente - 1].schieramento != schieramento))
                r.Add(new Vector2Int(XCorrente, YCorrente - 1));

            //Giù sinistra
            if (XCorrente - 1 >= 0)
            {
                if ((scacchiera[XCorrente - 1, YCorrente - 1] == null) || (scacchiera[XCorrente - 1, YCorrente - 1].schieramento != schieramento))
                    r.Add(new Vector2Int(XCorrente - 1, YCorrente - 1));
            }

            //Giù destra
            if (XCorrente + 1 < numeroCaselleX)
            {
                if ((scacchiera[XCorrente + 1, YCorrente - 1] == null) || (scacchiera[XCorrente + 1, YCorrente - 1].schieramento != schieramento))
                    r.Add(new Vector2Int(XCorrente + 1, YCorrente - 1));
            }
        }

        //Su
        if (YCorrente + 1 < numeroCaselleY)
        {
            if ((scacchiera[XCorrente, YCorrente + 1] == null) || (scacchiera[XCorrente, YCorrente + 1].schieramento != schieramento))
                r.Add(new Vector2Int(XCorrente, YCorrente + 1));

            //Su sinistra
            if (XCorrente - 1 >= 0)
            {
                if ((scacchiera[XCorrente - 1, YCorrente + 1] == null) || (scacchiera[XCorrente - 1, YCorrente + 1].schieramento != schieramento))
                    r.Add(new Vector2Int(XCorrente - 1, YCorrente + 1));
            }

            //Su destra
            if (XCorrente + 1 < numeroCaselleX)
            {
                if ((scacchiera[XCorrente + 1, YCorrente + 1] == null) || (scacchiera[XCorrente + 1, YCorrente + 1].schieramento != schieramento))
                    r.Add(new Vector2Int(XCorrente + 1, YCorrente + 1));
            }
        }

        //Destra
        if (XCorrente + 1 < numeroCaselleX)
        {
            if ((scacchiera[XCorrente + 1, YCorrente] == null) || (scacchiera[XCorrente + 1, YCorrente].schieramento != schieramento))
                r.Add(new Vector2Int(XCorrente + 1, YCorrente));
        }

        //Sinistra
        if (XCorrente - 1 >= 0)
        {
            if ((scacchiera[XCorrente - 1, YCorrente] == null) || (scacchiera[XCorrente - 1, YCorrente].schieramento != schieramento))
                r.Add(new Vector2Int(XCorrente - 1, YCorrente));
        }

        return r;
    }

    public override MossaSpeciale OttieniMosseSpeciali(ref Pezzo[,] scacchiera, ref List<Vector2Int[]> listaMosse, ref List<Vector2Int> mosseDisponibili)
    {
        MossaSpeciale r = MossaSpeciale.Nessuna;

        // cerco nell'arrai delle mosse se ce n'è una con re o torri come pezzo mosso, basta cercare le loro posizioni iniziali
        var mossaRe = listaMosse.Find(m => m[0].x == 4 && m[0].y == ((schieramento == 0) ? 0 : 7));
        var torreSinistra = listaMosse.Find(m => m[0].x == 0 && m[0].y == ((schieramento == 0) ? 0 : 7));
        var torreDestra = listaMosse.Find(m => m[0].x == 7 && m[0].y == ((schieramento == 0) ? 0 : 7));

        if (mossaRe == null)
        {
            if(schieramento == 0)
            {
                //arrocco con torre sinistra
                if (torreSinistra == null)
                    if (scacchiera[0,0].tipo == TipoPezzo.Torre)
                        if(scacchiera[0,0].schieramento == 0)
                            if (scacchiera[3, 0] == null && scacchiera[2, 0] == null && scacchiera[1, 0] == null)
                            {
                                mosseDisponibili.Add(new Vector2Int(2, 0));
                                r = MossaSpeciale.Arrocco;
                            }

                //arrocco con torre destra
                if (torreDestra == null)
                    if (scacchiera[7, 0].tipo == TipoPezzo.Torre)
                        if (scacchiera[7, 0].schieramento == 0)
                            if (scacchiera[6, 0] == null && scacchiera[5, 0] == null)
                            {
                                mosseDisponibili.Add(new Vector2Int(6, 0));
                                r = MossaSpeciale.Arrocco;
                            }
            }
            else
            {
                //arrocco con torre sinistra
                if (torreSinistra == null)
                    if (scacchiera[0, 7].tipo == TipoPezzo.Torre)
                        if (scacchiera[0, 7].schieramento == 1)
                            if (scacchiera[3, 7] == null && scacchiera[2, 7] == null && scacchiera[1, 7] == null)
                            {
                                mosseDisponibili.Add(new Vector2Int(2, 7));
                                r = MossaSpeciale.Arrocco;
                            }

                //arrocco con torre destra
                if (torreDestra == null)
                    if (scacchiera[7, 7].tipo == TipoPezzo.Torre)
                        if (scacchiera[7, 7].schieramento == 1)
                            if (scacchiera[6, 7] == null && scacchiera[5, 7] == null)
                            {
                                mosseDisponibili.Add(new Vector2Int(6, 7));
                                r = MossaSpeciale.Arrocco;
                            }
            }
        }
        return r;
    }

}
