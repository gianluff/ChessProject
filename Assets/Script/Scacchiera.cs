using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum MossaSpeciale
{
    Nessuna = 0,
    EnPassant,
    Arrocco,
    Promozione
}
public class Scacchiera : MonoBehaviour
{
    [Header("Componenti artistici")]
    [SerializeField] private Material materialeCasella;
    [SerializeField] private float dimCasella = 1.0f;
    [SerializeField] private float offsetY = 0.2f;
    [SerializeField] private float offsetBordo = 0.5f;
    [SerializeField] private Vector3 centroScacchiera = Vector3.zero;
    [SerializeField] private float dimensioneMangiati = 0.3f;
    [SerializeField] private float spazioMangiati = 0.3f;
    [SerializeField] private float OffsetSpostamento = 1.5f;
    [SerializeField] private GameObject SchermataVittoria;
    [Header("Prefab & Materiali")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] materialiSchieramento;
    /// <summary>
    [SerializeField] private Animator animazioneCamera;
    /// </summary>
    /// 
    [SerializeField] private GameObject gameUi;
    [SerializeField] private GameObject bottoneAbilita;
    [SerializeField] private GameObject bottoneRematch;
    [SerializeField] private GameObject bottoneExit;
    [SerializeField] private GameObject paBianchi;
    [SerializeField] private GameObject paNeri;

    //LOGICA
    private Pezzo[,] pezziScacchi;
    private Pezzo PezzoToccato;
    private const int NUM_CASELLE_X = 8;
    private const int NUM_CASELLE_Y = 8;
    private List<Pezzo> bianchiMangiati = new List<Pezzo>();
    private List<Pezzo> neriMangiati = new List<Pezzo>();
    private List<Vector2Int> mosseDisponibili = new List<Vector2Int>();
    private GameObject[,] caselle;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 limiti;
    private bool turnoArmataBianca;
    private MossaSpeciale mossaSpeciale;
    private List<Vector2Int[]> listaMosse = new List<Vector2Int[]>();
    /// 
    private bool abilita;
    private uint contatoreAPBianchi = 0;
    private uint contatoreAPNeri = 0;

    // Awake viene chiamata all'avvio del gioco
    void Awake()
    {
        turnoArmataBianca = true;
        GeneraCaselle(dimCasella, NUM_CASELLE_X, NUM_CASELLE_Y);
        SpawnPezzi();
        PosizionaPezzi();
    }
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        ///
        GameUI gui = gameUi.GetComponent<GameUI>();
        paBianchi.GetComponent<TextMeshProUGUI>().text = "PA Armata Bianca: " + contatoreAPBianchi;
        paNeri.GetComponent<TextMeshProUGUI>().text = "PA Armata Nera: " + contatoreAPNeri;
        if (gui.partitaIniziata)
        {
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Casella", "Hover", "Highlight")))
            {
                //mi permette di ottenere gli indici della casella su cui si trova il cursore
                Vector2Int hitPosition = LookupIndiceCasella(info.transform.gameObject);
                //Se non ho il focus su nessuna casella
                if (currentHover == -Vector2Int.one)
                {
                    currentHover = hitPosition;
                    caselle[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //se avevo gà il focus su un'altra casella la aggiorno riportando la precedente nel layer casella
                if (currentHover != hitPosition)
                {
                    caselle[currentHover.x, currentHover.y].layer = (ContieneMossaValida(ref mosseDisponibili, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Casella");
                    currentHover = hitPosition;
                    caselle[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //se clicco su un pezzo
                if (Input.GetMouseButtonDown(0))
                {
                    if (pezziScacchi[hitPosition.x, hitPosition.y] != null)
                    {
                        //è il mio turno?
                        if ((pezziScacchi[hitPosition.x, hitPosition.y].schieramento == 0 && turnoArmataBianca) || (pezziScacchi[hitPosition.x, hitPosition.y].schieramento == 1 && !turnoArmataBianca))
                        {
                            PezzoToccato = pezziScacchi[hitPosition.x, hitPosition.y];

                            //ottengo una lista delle mosse disponibili
                            mosseDisponibili = PezzoToccato.VediMosseDisponibili(ref pezziScacchi, NUM_CASELLE_X, NUM_CASELLE_Y, abilita);
                            //ottengo una lista delle mosse speciali disponibili
                            if(!abilita)
                                mossaSpeciale = PezzoToccato.OttieniMosseSpeciali(ref pezziScacchi, ref listaMosse, ref mosseDisponibili);
                            


                            PrevieniScacco();
                            EvidenziaCaselle();
                        }
                    }
                }

                if (PezzoToccato != null && Input.GetMouseButtonUp(0))
                {
                    Vector2Int posizionePrecedente = new Vector2Int(PezzoToccato.XCorrente, PezzoToccato.YCorrente);

                    bool MossaValida = MuoviVerso(PezzoToccato, hitPosition.x, hitPosition.y);
                    if (!MossaValida)
                        PezzoToccato.SetPosizione(OttieniCentroCasella(posizionePrecedente.x, posizionePrecedente.y));
                    PezzoToccato = null;
                    RimuoviEvidenziaCaselle();
                }

            }
            else
            { //Se sono fuori dalla scacchiera riporto l'ultima casella "toccata" al layer originale e setto currentHover a -1
                if (currentHover == -Vector2Int.one)
                {
                    caselle[currentHover.x, currentHover.y].layer = (ContieneMossaValida(ref mosseDisponibili, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Casella");
                    currentHover = -Vector2Int.one;
                }
                if (PezzoToccato != null && Input.GetMouseButtonUp(0))
                {
                    PezzoToccato.SetPosizione(OttieniCentroCasella(PezzoToccato.XCorrente, PezzoToccato.YCorrente));
                    PezzoToccato = null;
                    RimuoviEvidenziaCaselle();
                }

            }
            //se sto spostando un pezzo
            if (PezzoToccato)
            {
                Plane pianoOrizzontale = new Plane(Vector3.up, Vector3.up * offsetY);
                float distanza = 0.0f;
                if (pianoOrizzontale.Raycast(ray, out distanza))
                    PezzoToccato.SetPosizione(ray.GetPoint(distanza) + Vector3.up * OffsetSpostamento);
            }
            ///
        }
    }
 
    // Generacaselle crea una matrice di caselle che saranno la scacchiera
    // nei du cicli for si genera una casella alla volta
    private void GeneraCaselle(float dimensioneCasella, int numCaselleX, int numCaselleY)
    {
        offsetY += transform.position.y;
        limiti = new Vector3((numCaselleX / 2) * dimensioneCasella, 0, (numCaselleX / 2) * dimensioneCasella) + centroScacchiera;
        caselle = new GameObject[numCaselleX, numCaselleY];
        for (int x = 0; x < numCaselleX; x++)
                    for (int y = 0; y < numCaselleY; y++)
                            caselle[x, y] = GeneraSingolaCasella(dimensioneCasella, x, y);
            
        
    }

    //Genero le singole caselle, associando a ognuna una mesh
    private GameObject GeneraSingolaCasella(float dimensioneCasella, int x, int y)
    {
        GameObject casellaObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        casellaObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        casellaObject.AddComponent<MeshFilter>().mesh = mesh;
        //Assegno un materiale alla mesh. Il materiale è associato direttamente su Unity nella scheda inspector
        casellaObject.AddComponent<MeshRenderer>().material = materialeCasella;

        //creo i vertici per i 4 angoli della singola casella
        Vector3[] vertici = new Vector3[4];
        vertici[0] = new Vector3(x * dimensioneCasella, offsetY, y * dimensioneCasella) - limiti;
        vertici[1] = new Vector3(x * dimensioneCasella, offsetY, (y+1) * dimensioneCasella) - limiti;
        vertici[2] = new Vector3((x+1) * dimensioneCasella, offsetY, y  * dimensioneCasella) - limiti;
        vertici[3] = new Vector3((x+1) * dimensioneCasella, offsetY, (y+1) * dimensioneCasella) - limiti;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        //assegno le coordinate dei vertici alla mesh ed eseguo una triangolazione
        //il primo triangolo è formato da vertici 0, 1 e 2 e il secondo dai vertici 1, 3 e 2
        mesh.vertices = vertici;
        mesh.triangles = tris;

        //calcola appropriatamente la luce sopra la mesh
        mesh.RecalculateNormals();

        casellaObject.layer = LayerMask.NameToLayer("Casella");
        casellaObject.AddComponent<BoxCollider>();
        return casellaObject;
    }

    //SPawning dei pezzi
    private void SpawnPezzi()
    {
        pezziScacchi = new Pezzo[NUM_CASELLE_X, NUM_CASELLE_Y];
        int armataBianca = 0;
        int armataNera = 1;

        //Squadra bianca
        pezziScacchi[0, 0] = SpawnSingoloPezzo(TipoPezzo.Torre, armataBianca);
        pezziScacchi[1, 0] = SpawnSingoloPezzo(TipoPezzo.Cavallo, armataBianca);
        pezziScacchi[2, 0] = SpawnSingoloPezzo(TipoPezzo.Alfiere, armataBianca);
        pezziScacchi[3, 0] = SpawnSingoloPezzo(TipoPezzo.Regina, armataBianca);
        pezziScacchi[4, 0] = SpawnSingoloPezzo(TipoPezzo.Re, armataBianca);
        pezziScacchi[5, 0] = SpawnSingoloPezzo(TipoPezzo.Alfiere, armataBianca);
        pezziScacchi[6, 0] = SpawnSingoloPezzo(TipoPezzo.Cavallo, armataBianca);
        pezziScacchi[7, 0] = SpawnSingoloPezzo(TipoPezzo.Torre, armataBianca);
        for (int i = 0; i < NUM_CASELLE_X; i++)
        {
            pezziScacchi[i, 1] = SpawnSingoloPezzo(TipoPezzo.Pedone, armataBianca);
        }

        pezziScacchi[0, 7] = SpawnSingoloPezzo(TipoPezzo.Torre, armataNera);
        pezziScacchi[1, 7] = SpawnSingoloPezzo(TipoPezzo.Cavallo, armataNera);
        pezziScacchi[2, 7] = SpawnSingoloPezzo(TipoPezzo.Alfiere, armataNera);
        pezziScacchi[3, 7] = SpawnSingoloPezzo(TipoPezzo.Regina, armataNera);
        pezziScacchi[4, 7] = SpawnSingoloPezzo(TipoPezzo.Re, armataNera);
        pezziScacchi[5, 7] = SpawnSingoloPezzo(TipoPezzo.Alfiere, armataNera);
        pezziScacchi[6, 7] = SpawnSingoloPezzo(TipoPezzo.Cavallo, armataNera);
        pezziScacchi[7, 7] = SpawnSingoloPezzo(TipoPezzo.Torre, armataNera);
        for (int i = 0; i < NUM_CASELLE_Y; i++)
        {
            pezziScacchi[i, 6] = SpawnSingoloPezzo(TipoPezzo.Pedone, armataNera);
        }

    }
    private Pezzo SpawnSingoloPezzo(TipoPezzo tipo, int armata)
    {
        Pezzo pz = Instantiate(prefabs[(int)tipo - 1], transform).GetComponent<Pezzo>();
        pz.tipo = tipo;
        pz.schieramento = armata;
        pz.GetComponent<MeshRenderer>().material = materialiSchieramento[armata];
        return pz;
    }

    //Posizionamento pezzi
    private void PosizionaPezzi()
    {
        for (int x = 0; x < NUM_CASELLE_X; x++)
        {
            for (int y = 0; y < NUM_CASELLE_X; y++)
            {
                if (pezziScacchi[x, y] != null)
                    PosizionaSingoloPezzo(x, y, true);
            }
        }
    }
    private void PosizionaSingoloPezzo(int x, int y, bool force = false)
    {
        pezziScacchi[x, y].XCorrente = x;
        pezziScacchi[x, y].YCorrente = y;
        //pezziScacchi[x, y].transform.position = new Vector3(x * dimCasella, offsetY, y * dimCasella);
        //pezziScacchi[x, y].SetPosizione(new Vector3(x * dimCasella, offsetY, y * dimCasella));
        pezziScacchi[x, y].SetPosizione(OttieniCentroCasella(x, y), force);
    }
    private Vector3 OttieniCentroCasella(int x, int y)
    {
        return new Vector3(x * dimCasella, offsetY, y * dimCasella) - limiti + new Vector3(dimCasella / 2, 0, dimCasella / 2);
    }

    //Highlight caselle
    private void EvidenziaCaselle()
    {
        for (int i = 0; i < mosseDisponibili.Count; i++)
        {
            caselle[mosseDisponibili[i].x, mosseDisponibili[i].y].layer = LayerMask.NameToLayer("Highlight");
            //caselle[mosseDisponibili[i].x, mosseDisponibili[i].y].layer = 8;
        }
    }
    private void RimuoviEvidenziaCaselle()
    {
        for (int i = 0; i < mosseDisponibili.Count; i++)
        {
            caselle[mosseDisponibili[i].x, mosseDisponibili[i].y].layer = LayerMask.NameToLayer("Casella");
        }
        mosseDisponibili.Clear();
    }

    //Scacco matto
    private void ScaccoMatto(int armata)
    {
        MostraVittoria(armata);
    }
    private void MostraVittoria(int armataVincente)
    {
        SchermataVittoria.SetActive(true);
        SchermataVittoria.transform.GetChild(armataVincente).gameObject.SetActive(true);
        ///
        bottoneAbilita.SetActive(false);
        bottoneRematch.SetActive(false);
        bottoneExit.SetActive(false);

    }
    public void OnResetButton()
    {
        SchermataVittoria.transform.GetChild(0).gameObject.SetActive(false);
        SchermataVittoria.transform.GetChild(1).gameObject.SetActive(false);
        SchermataVittoria.SetActive(false);

        PezzoToccato = null;
        mosseDisponibili.Clear();
        listaMosse.Clear();

        for (int i = 0; i < NUM_CASELLE_X; i++)
        {
            for (int j = 0; j < NUM_CASELLE_Y; j++)
            {
                if (pezziScacchi[i, j] != null)
                    Destroy(pezziScacchi[i, j].gameObject);

                pezziScacchi[i, j] = null;
            }
        }

        for (int i = 0; i < bianchiMangiati.Count; i++)
            Destroy(bianchiMangiati[i].gameObject);
        for (int i = 0; i < neriMangiati.Count; i++)
            Destroy(neriMangiati[i].gameObject);

        bianchiMangiati.Clear();
        neriMangiati.Clear();

        SpawnPezzi();
        PosizionaPezzi();
        turnoArmataBianca = true;
        ///
        contatoreAPBianchi = 0;
        contatoreAPNeri = 0;
        animazioneCamera.SetTrigger("Base");
        bottoneAbilita.SetActive(true);
        bottoneRematch.SetActive(true);
        bottoneExit.SetActive(true);
    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    /// 

    public void OnExitInGameButton()
    {
        Application.Quit();
    }

    public void OnSkillButton()
    {
        if (turnoArmataBianca && contatoreAPBianchi >= 4 || !turnoArmataBianca && contatoreAPNeri >= 4)
        {
            abilita = !abilita;
            if (abilita)
                bottoneAbilita.GetComponent<Image>().color = Color.green;
            else
                bottoneAbilita.GetComponent<Image>().color = Color.white;
        }
    }

    public void OnRematchButton()
    {
        PezzoToccato = null;
        mosseDisponibili.Clear();
        listaMosse.Clear();

        for (int i = 0; i < NUM_CASELLE_X; i++)
        {
            for (int j = 0; j < NUM_CASELLE_Y; j++)
            {
                if (pezziScacchi[i, j] != null)
                    Destroy(pezziScacchi[i, j].gameObject);

                pezziScacchi[i, j] = null;
            }
        }

        for (int i = 0; i < bianchiMangiati.Count; i++)
            Destroy(bianchiMangiati[i].gameObject);
        for (int i = 0; i < neriMangiati.Count; i++)
            Destroy(neriMangiati[i].gameObject);

        bianchiMangiati.Clear();
        neriMangiati.Clear();

        SpawnPezzi();
        PosizionaPezzi();
        turnoArmataBianca = true;
        ///
        animazioneCamera.SetTrigger("Base");
        contatoreAPBianchi = 0;
        contatoreAPNeri = 0;
        bottoneAbilita.SetActive(true);
        bottoneRematch.SetActive(true);
        bottoneExit.SetActive(true);
    }

    //Mosse speciali
    private void ProcessaMossaSpeciale()
    {
        if (mossaSpeciale == MossaSpeciale.EnPassant)
        {
            var nuovaMossa = listaMosse[listaMosse.Count - 1];
            Pezzo mioPedone = pezziScacchi[nuovaMossa[1].x, nuovaMossa[1].y];
            var posizioneTargetPedone = listaMosse[listaMosse.Count - 2];
            Pezzo pedoneNemico = pezziScacchi[posizioneTargetPedone[1].x, posizioneTargetPedone[1].y];

            if (mioPedone.XCorrente == pedoneNemico.XCorrente)
            {
                if (mioPedone.YCorrente == pedoneNemico.YCorrente - 1 || mioPedone.YCorrente == pedoneNemico.YCorrente + 1)
                {
                    if (pedoneNemico.schieramento == 0)
                    {
                        bianchiMangiati.Add(pedoneNemico);
                        pedoneNemico.SetProporzione(Vector3.one * dimensioneMangiati);
                        pedoneNemico.SetPosizione(new Vector3(8 * dimCasella, offsetBordo, -1 * dimCasella)
                                        - limiti
                                        + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                        + Vector3.back * spazioMangiati * bianchiMangiati.Count);
                    }
                    else
                    {
                        neriMangiati.Add(pedoneNemico);
                        pedoneNemico.SetProporzione(Vector3.one * dimensioneMangiati);
                        pedoneNemico.SetPosizione(new Vector3(-1 * dimCasella, offsetBordo, 8 * dimCasella)
                                        - limiti
                                        + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                        + Vector3.back * spazioMangiati * neriMangiati.Count);
                    }
                    pezziScacchi[pedoneNemico.XCorrente, pedoneNemico.YCorrente] = null;
                }
            }
        }

        if (mossaSpeciale == MossaSpeciale.Promozione)
        {
            Vector2Int[] ultimaMossa = listaMosse[listaMosse.Count - 1];
            Pezzo pedoneTarget = pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y];

            if (pedoneTarget.tipo == TipoPezzo.Pedone)
            {
                if (pedoneTarget.schieramento == 0 && ultimaMossa[1].y == 7)
                {
                    Pezzo nuovaRegina = SpawnSingoloPezzo(TipoPezzo.Regina, 0);
                    nuovaRegina.transform.position = pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y].transform.position;
                    Destroy(pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y].gameObject);
                    pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y] = nuovaRegina;
                    PosizionaSingoloPezzo(ultimaMossa[1].x, ultimaMossa[1].y);
                }
                if (pedoneTarget.schieramento == 1 && ultimaMossa[1].y == 0)
                {
                    Pezzo nuovaRegina = SpawnSingoloPezzo(TipoPezzo.Regina, 1);
                    Destroy(pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y].gameObject);
                    pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y] = nuovaRegina;
                    PosizionaSingoloPezzo(ultimaMossa[1].x, ultimaMossa[1].y, true);
                }


            }
        }

        if (mossaSpeciale == MossaSpeciale.Arrocco)
        {
            Vector2Int[] ultimaMossa = listaMosse[listaMosse.Count - 1];
            //torre sinistra
            if(ultimaMossa[1].x == 2)
            {
                //arrocco bianco
                if (ultimaMossa[1].y == 0)
                {
                    Pezzo torre = pezziScacchi[0, 0];
                    pezziScacchi[3, 0] = torre;
                    PosizionaSingoloPezzo(3, 0);
                    pezziScacchi[0, 0] = torre;
                }
                //arrocco nero
                else if (ultimaMossa[1].y == 7)
                {
                    Pezzo torre = pezziScacchi[0, 7];
                    pezziScacchi[3, 7] = torre;
                    PosizionaSingoloPezzo(3, 7);
                    pezziScacchi[0, 7] = torre;
                }

            }
            else if (ultimaMossa[1].x == 6)
            {
                //arrocco bianco
                if (ultimaMossa[1].y == 0)
                {
                    Pezzo torre = pezziScacchi[7, 0];
                    pezziScacchi[5, 0] = torre;
                    PosizionaSingoloPezzo(5, 0);
                    pezziScacchi[7, 0] = torre;
                }
                //arrocco nero
                else if (ultimaMossa[1].y == 7)
                {
                    Pezzo torre = pezziScacchi[7, 7];
                    pezziScacchi[5, 7] = torre;
                    PosizionaSingoloPezzo(5, 7);
                    pezziScacchi[5, 7] = torre;
                }
            }
        }
    }
    private void PrevieniScacco()
    {
        Pezzo reTarget = null;
        for (int x = 0; x < NUM_CASELLE_X; x++)
            for (int y = 0; y < NUM_CASELLE_Y; y++)
                if (pezziScacchi[x, y] != null)
                    if (pezziScacchi[x, y].tipo == TipoPezzo.Re)
                        if (pezziScacchi[x, y].schieramento == PezzoToccato.schieramento)
                            reTarget = pezziScacchi[x, y];

        SimulaMossaPerSingoloPezzo(PezzoToccato, ref mosseDisponibili, reTarget);
    }
    private void SimulaMossaPerSingoloPezzo(Pezzo p, ref List<Vector2Int> mosse, Pezzo reTarget)
    {
        //passo le mossedisponibili con una reference in modo da poter eliminare dalla lista quelle che porterebbero a uno scacco
        //la funzione simula le mosse alla ricerca di un possibile scacco, quindi prima salvo
        //i valori correnti per resettarli dopo la chiamata
        int xAttuale = p.XCorrente;
        int yAttuale = p.YCorrente;
        List<Vector2Int> mosseDaRimuovere = new List<Vector2Int>();

        //simulazione delle mosse e verifica di eventuale scacco
        for (int i = 0; i < mosse.Count; i++)
        {
            int simX = mosse[i].x;
            int simY = mosse[i].y;

            Vector2Int posizioneReSimulata = new Vector2Int(reTarget.XCorrente, reTarget.YCorrente);
            //valuto se il pezzo che sto provando a muovere è proprio il re
            if (p.tipo == TipoPezzo.Re)
                posizioneReSimulata = new Vector2Int(simX, simY);

            //simulo la modifica della scacchiera, copio la scacchiera attuale senza usare una reference
            Pezzo[,] simulazione = new Pezzo[NUM_CASELLE_X, NUM_CASELLE_Y];
            //lista dei pezzi che mi potrebbero attaccare al prossimo turno
            List<Pezzo> pezziAttaccantiSimulati = new List<Pezzo>();
            for (int x = 0; x < NUM_CASELLE_X; x++)
                for (int y = 0; y < NUM_CASELLE_Y; y++)
                    if (pezziScacchi[x, y] != null) 
                    {
                        simulazione[x, y] = pezziScacchi[x, y];
                        if (simulazione[x, y].schieramento != p.schieramento)
                            pezziAttaccantiSimulati.Add(simulazione[x, y]);
                    }
            //effettiva simulazione della mossa
            simulazione[xAttuale, yAttuale] = null;
            p.XCorrente = simX;
            p.YCorrente = simY;
            simulazione[simX, simY] = p;

            //uno dei nostri pezzi è stato buttato giù nella simulazione?
            var pezzoMangiato = pezziAttaccantiSimulati.Find(c => c.XCorrente == simX && c.YCorrente == simY);
            if (pezzoMangiato != null)
                pezziAttaccantiSimulati.Remove(pezzoMangiato);

            //otteniamo le liste degli attacchi simulati di tutti i pezzi nemici
            List<Vector2Int> mosseSim = new List<Vector2Int>();
            for (int a = 0; a < pezziAttaccantiSimulati.Count; a++)
            {
                var mossePezzi = pezziAttaccantiSimulati[a].VediMosseDisponibili(ref simulazione, NUM_CASELLE_X, NUM_CASELLE_Y, abilita);
                for (int b = 0; b < mossePezzi.Count; b++)
                    mosseSim.Add(mossePezzi[b]);

            }

            //se una delle mosse ha il re come casella di arrivo segnala quella mossa come da rimuovere
            if (ContieneMossaValida(ref mosseSim, posizioneReSimulata))
            {
                mosseDaRimuovere.Add(mosse[i]);
            }

            //resetto i dati del pezzo toccato
            p.XCorrente = xAttuale;
            p.YCorrente = yAttuale;
        }

        //rimozione delle mosse
        for (int i = 0; i < mosseDaRimuovere.Count; i++)
            mosse.Remove(mosseDaRimuovere[i]);
        

    }
    private bool ScaccoPerScaccoMatto()
    {
        var ultimaMossa = listaMosse[listaMosse.Count - 1];
        int armataTarget = (pezziScacchi[ultimaMossa[1].x, ultimaMossa[1].y].schieramento == 0) ? 1 : 0;
        List<Pezzo> pezziAttaccanti = new List<Pezzo>();
        List<Pezzo> pezziDifensori = new List<Pezzo>();
        Pezzo reTarget = null;
        for (int x = 0; x < NUM_CASELLE_X; x++){
            for (int y = 0; y < NUM_CASELLE_X; y++){
                if (pezziScacchi[x, y] != null)
                {
                    if (pezziScacchi[x, y].schieramento == armataTarget)
                    {
                        pezziDifensori.Add(pezziScacchi[x, y]);
                        if (pezziScacchi[x, y].tipo == TipoPezzo.Re)
                            reTarget = pezziScacchi[x, y];
                    }
                    else
                    {
                        pezziAttaccanti.Add(pezziScacchi[x, y]);
                    }
                }
            }
        }

        List<Vector2Int> mosseDisponibiliCorrenti = new List<Vector2Int>();
        for (int a = 0; a < pezziAttaccanti.Count; a++)
        {
            var mossePezzi = pezziAttaccanti[a].VediMosseDisponibili(ref pezziScacchi, NUM_CASELLE_X, NUM_CASELLE_Y, abilita);
            for (int b = 0; b < mossePezzi.Count; b++)
                mosseDisponibiliCorrenti.Add(mossePezzi[b]);
        }

        //siamo in scacco?
        if(ContieneMossaValida(ref mosseDisponibiliCorrenti, new Vector2Int(reTarget.XCorrente, reTarget.YCorrente)))
        {
            //se il re è in scacco valutiamo se possiamo muovere qualcosa per difenderlo
            for(int i = 0; i < pezziDifensori.Count; i++)
            {
                List<Vector2Int> mosseDifensive = pezziDifensori[i].VediMosseDisponibili(ref pezziScacchi, NUM_CASELLE_X, NUM_CASELLE_Y, abilita);
                SimulaMossaPerSingoloPezzo(pezziDifensori[i], ref mosseDifensive, reTarget);

                if (mosseDifensive.Count != 0)
                    return false;
            }

            return true; //uscita per lo scacco matto
        }
        return false;
    }

    //Operazioni
    private bool ContieneMossaValida(ref List<Vector2Int> mosse, Vector2 pos)
    {
        for (int i = 0; i < mosse.Count; i++)
        {
            if (mosse[i].x == pos.x && mosse[i].y == pos.y)
                return true;
        }
        return false;
    }
    private bool MuoviVerso(Pezzo pt, int x, int y)
    {

        if (!ContieneMossaValida(ref mosseDisponibili, new Vector2(x, y)))
            return false;

        //verificare se sulla posizione target c'è già un pezzo
        if (!abilita)
        {
            if (pezziScacchi[x, y] != null)
            {
                Pezzo op = pezziScacchi[x, y];
                if (pt.schieramento == op.schieramento)
                    return false;

                //Se i pezzi sono di armate diverse
                if (op.schieramento == 0)

                {
                    if (op.tipo == TipoPezzo.Re)
                        ScaccoMatto(1);
                    bianchiMangiati.Add(op);
                    op.SetProporzione(Vector3.one * dimensioneMangiati);
                    op.SetPosizione(new Vector3(8 * dimCasella, offsetBordo, -1 * dimCasella)
                                    - limiti
                                    + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                    + Vector3.forward * spazioMangiati * bianchiMangiati.Count);
                    if (op.tipo == TipoPezzo.Pedone)
                        contatoreAPBianchi++;
                    else if (op.tipo == TipoPezzo.Alfiere || op.tipo == TipoPezzo.Cavallo)
                        contatoreAPBianchi = contatoreAPBianchi + 3;
                    else if (op.tipo == TipoPezzo.Torre)
                        contatoreAPBianchi = contatoreAPBianchi + 5;
                    else if (op.tipo == TipoPezzo.Regina)
                        contatoreAPBianchi = contatoreAPBianchi + 10;
                }

                else
                {
                    if (op.tipo == TipoPezzo.Re)
                        ScaccoMatto(0);
                    neriMangiati.Add(op);
                    op.SetProporzione(Vector3.one * dimensioneMangiati);
                    op.SetPosizione(new Vector3(-1 * dimCasella, offsetBordo, 8 * dimCasella)
                                    - limiti
                                    + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                    + Vector3.back * spazioMangiati * neriMangiati.Count);
                    if (op.tipo == TipoPezzo.Pedone)
                        contatoreAPNeri++;
                    else if (op.tipo == TipoPezzo.Alfiere || op.tipo == TipoPezzo.Cavallo)
                        contatoreAPNeri = contatoreAPNeri + 3;
                    else if (op.tipo == TipoPezzo.Torre)
                        contatoreAPNeri = contatoreAPNeri + 5;
                    else if (op.tipo == TipoPezzo.Regina)
                        contatoreAPNeri = contatoreAPNeri + 10;
                }

            }
        }
        ///Berserk mangia tutti i pezzi alleati o nemici nella direzione scelta
        if(pt.tipo == TipoPezzo.Torre && abilita == true)
        {
            Debug.Log("Eccoci");
            if (pt.XCorrente == x)
            {
                if (y == 7)
                {
                    for (int j = y; j >= pt.YCorrente +1; j--)
                    {
                        if (pezziScacchi[x, j] != null)
                        {
                            Pezzo op = pezziScacchi[x, j];
                            if (op.schieramento == 0)
                            {
                                bianchiMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(8 * dimCasella, offsetBordo, -1 * dimCasella)
                                            - limiti
                                            + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                            + Vector3.forward * spazioMangiati * bianchiMangiati.Count);
                            }
                            if (op.schieramento == 1)
                            {
                                neriMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(-1 * dimCasella, offsetBordo, 8 * dimCasella)
                                    - limiti
                                    + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                    + Vector3.back * spazioMangiati * neriMangiati.Count);
                            }
                            pezziScacchi[x, j] = null;
                        }
                    }
                }

                if (y == 0)
                {
                    for (int j = y; j <= pt.YCorrente -1; j++)
                    {
                        if (pezziScacchi[x, j] != null)
                        {
                            Pezzo op = pezziScacchi[x, j];
                            if (op.schieramento == 0)
                            {
                                bianchiMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(8 * dimCasella, offsetBordo, -1 * dimCasella)
                                            - limiti
                                            + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                            + Vector3.forward * spazioMangiati * bianchiMangiati.Count);
                            }
                            if (op.schieramento == 1)
                            {
                                neriMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(-1 * dimCasella, offsetBordo, 8 * dimCasella)
                                    - limiti
                                    + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                    + Vector3.back * spazioMangiati * neriMangiati.Count);
                            }
                            pezziScacchi[x, j] = null;
                        }
                    }
                }
            }

            if (pt.YCorrente == y)
            {
                if (x == 7)
                {
                    for (int i = x; i >= pt.XCorrente +1; i--)
                    {
                        if (pezziScacchi[i, y] != null)
                        {
                            Pezzo op = pezziScacchi[i, y];
                            if (op.schieramento == 0)
                            {
                                bianchiMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(8 * dimCasella, offsetBordo, -1 * dimCasella)
                                            - limiti
                                            + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                            + Vector3.forward * spazioMangiati * bianchiMangiati.Count);
                            }
                            if (op.schieramento == 1)
                            {
                                neriMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(-1 * dimCasella, offsetBordo, 8 * dimCasella)
                                    - limiti
                                    + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                    + Vector3.back * spazioMangiati * neriMangiati.Count);
                            }
                            pezziScacchi[i, y] = null;
                        }
                    }
                }

                if (x == 0)
                {
                    for (int i = x; i <= pt.XCorrente-1; i++)
                    {
                        if (pezziScacchi[i, y] != null)
                        {
                            Pezzo op = pezziScacchi[i, y];
                            if (op.schieramento == 0)
                            {
                                bianchiMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(8 * dimCasella, offsetBordo, -1 * dimCasella)
                                            - limiti
                                            + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                            + Vector3.forward * spazioMangiati * bianchiMangiati.Count);
                            }
                            if (op.schieramento == 1)
                            {
                                neriMangiati.Add(op);
                                op.SetProporzione(Vector3.one * dimensioneMangiati);
                                op.SetPosizione(new Vector3(-1 * dimCasella, offsetBordo, 8 * dimCasella)
                                    - limiti
                                    + new Vector3(dimCasella / 2, 0, dimCasella / 2)
                                    + Vector3.back * spazioMangiati * neriMangiati.Count);
                            }
                            pezziScacchi[i, y] = null;
                        }
                    }
                }
            }
            if (turnoArmataBianca)
                contatoreAPBianchi = contatoreAPBianchi - 4;
            else
                contatoreAPNeri = contatoreAPNeri - 4;
            
        }///

        Vector2Int posizionePrecedente = new Vector2Int(pt.XCorrente, pt.YCorrente);
        pezziScacchi[x, y] = pt;
        pezziScacchi[posizionePrecedente.x, posizionePrecedente.y] = null;
        PosizionaSingoloPezzo(x, y);
        turnoArmataBianca = !turnoArmataBianca;
        listaMosse.Add(new Vector2Int[] { posizionePrecedente, new Vector2Int(x, y) });
        ProcessaMossaSpeciale();
        if (ScaccoPerScaccoMatto())
            ScaccoMatto(pt.schieramento);
        //////////
        if (turnoArmataBianca)
            animazioneCamera.SetTrigger("Base");
        else
            animazioneCamera.SetTrigger("ArmataNera");
        abilita = false;
        bottoneAbilita.GetComponent<Image>().color = Color.white;
        //////////
        return true;
    }
    private Vector2Int LookupIndiceCasella(GameObject hitInfo)
    {
        for (int x = 0; x < NUM_CASELLE_X; x++)
        {
            for (int y = 0; y < NUM_CASELLE_Y; y++)
            {
                if (caselle[x, y] == hitInfo)
                    return new Vector2Int(x, y);
            }
        }
        return -Vector2Int.one;
    }

}

