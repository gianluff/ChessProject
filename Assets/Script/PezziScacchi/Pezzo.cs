using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoPezzo
{
    Nessuno = 0,
    Pedone = 1,
    Torre = 2,
    Cavallo = 3,
    Alfiere = 4,
    Regina = 5,
    Re = 6
}
public class Pezzo : MonoBehaviour
{
    public int schieramento;
    public int XCorrente;
    public int YCorrente;
    public TipoPezzo tipo;
    private Vector3 posizioneDesiderata;
    private Vector3 proporzioneDesiderata = Vector3.one;

    private void Start()
    {
        transform.rotation = Quaternion.Euler((schieramento == 0) ? Vector3.zero : new Vector3(0, 180, 0));
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, posizioneDesiderata, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, proporzioneDesiderata, Time.deltaTime * 10);
    }

    public virtual List<Vector2Int> VediMosseDisponibili(ref Pezzo[,] scacchiera, int numeroCaselleX, int numeroCaselleY, bool skill)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));
        return r;
    }
    public virtual MossaSpeciale OttieniMosseSpeciali(ref Pezzo[,] scacchiera, ref List<Vector2Int[]> listaMosse, ref List<Vector2Int> mosseDisponibili)
    {
        return MossaSpeciale.Nessuna;
    }
    public virtual void SetPosizione(Vector3 posizione, bool force = false)
    {
        posizioneDesiderata = posizione;
        if (force)
            transform.position = posizioneDesiderata;
    }
    public virtual void SetProporzione(Vector3 proporzione, bool force = false)
    {
        proporzioneDesiderata = proporzione;
        if (force)
            transform.localScale = proporzioneDesiderata;
    }
}
