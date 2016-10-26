using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour {

	public float velocity; // velocidade em x do player
	public float bulletMaxInitialVelocity; // velocidade inicial da bala
	public float maxTimeShooting; // maximo tempo atirando
	public BoxCollider2D groundBC; // ref para o BoxCollider2D do chao
	public GameObject bulletPrefab; // ref para o GameObject ( Pre-fabricado ) da nossa bala
    public GameObject graneadeprefab;

	private BoxCollider2D bc; // ref para o BoxCollider2D do player
	private Rigidbody2D rb; // ref para o Rigidbody2D do player
	private Animator an; // ref para o Animator do GameObject Body
	private bool shooting; // o Player esta atirando?
	private float timeShooting; // tempo que o player esta atirando
	private Vector2 shootDirection; // ref para Vector2 normalizado que aponta na direçao do tiro do nosso player

	public GameObject shootingEffect; // ref para o GameObject que contem o efeito de particula do Player atirando
	public Transform gunTransform; // ref para o Transform do GameObject Gun ( Gun contem a sprite da arma e da mira )
    public Transform granadetransform;
    public Transform bodyTransform; // ref para o Transform do GameObject Body ( Body contem a sprite do corpo da minhoca )
	public Transform bulletInitialTransform; // ref para o Transform que guarda a posiçao inicial da bala
    public static int weapon;
	private bool targetting; // o player esta mirando?

	// Use this for initialization
	void Start () {
		bc = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		// Procurando por um component do tipo Animator nos GameObjects filhos de Player 
		// Na verdade queremos o componente Animator que esta no GameObject Body
		an = GetComponentInChildren<Animator>();
		//gunTransform.eulerAngles = new Vector3(0f, 0f, -30f);
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = Vector2.right * velocity;
            if (bodyTransform.localScale.x > 0f)
                bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);

            an.SetBool("moving", true);
        }
        if (!Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = -Vector2.right * velocity;
            if (bodyTransform.localScale.x < 0f)
                bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);

            an.SetBool("moving", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {

            rb.AddForce(new Vector2(0, 400));
           
            an.SetBool("moving", false);
        }

        if (rb.velocity == new Vector2(0, 0))
        {
            an.SetBool("moving", false);
        }




        if ( Input.GetKeyDown(KeyCode.W) ){ // A arma se torna visivel
			targetting = true;
            weapon = 1;
			gunTransform.gameObject.SetActive(true);
            granadetransform.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        { // A arma se torna visivel
            targetting = true;
            weapon = 2;
            granadetransform.gameObject.SetActive(true);
            gunTransform.gameObject.SetActive(false);
        }
        if ( targetting ){
			UpdateTargetting();
			UpdateShootDetection();
			if( shooting )
				UpdateShooting();
		}
		//gunTransform.localEulerAngles = new Vector3(0f, 0f, 30f);
	}

	// Verifica se o Player começou atirar
	void UpdateShootDetection(){
		// GetKeyDown retorna true apenas no update em que o player aperta a tecla
		// GetKey retorna true enquanto a tecla estiver pressionada
		// GetKeyUp retorna true no update em que o player solta a tecla
		if( Input.GetMouseButtonDown(0)){
			shooting = true;
            if (weapon == 1)
            {
                shootingEffect.SetActive(true);
            }
			timeShooting = 0f;
		}
	}

	// Caso o Player esteja atirando, marca a qto tempo o Plyer esta atirando e verifica
	// Se o Player parou de atirar ou se ja passou o tempo limite de atirar
	// Tb chama a funçao Shoot(), que efetivamente efetua o disparo
	void UpdateShooting(){
		timeShooting += Time.deltaTime;
		if(  Input.GetMouseButtonUp(0) ){
			shooting = false;
			shootingEffect.SetActive(false);
			Shoot();
		}
		if( timeShooting > maxTimeShooting ){
			shooting = false;
			shootingEffect.SetActive(false);
			Shoot ();
		}
	}

	// Funçao que cria um GameObject Bullet a partir de bulletPrefab
	// Posiciona nova bala criada
	// E tb a direciona na direçao em que o player esta mirando:
	// Vector2 que tem como origem o player e destino a posiçao do mouse
	void Shoot(){
		Vector3 mousePosScreen = Input.mousePosition;
		Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
		Vector2 playerToMouse = new Vector2( mousePosWorld.x - transform.position.x,
		                                    mousePosWorld.y - transform.position.y);
		
		playerToMouse.Normalize();

		shootDirection = playerToMouse;
		Debug.Log("Shoot!");
        if (weapon == 1)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletInitialTransform.position;
            bullet.GetComponent<Rigidbody2D>().velocity = shootDirection * bulletMaxInitialVelocity * (timeShooting / maxTimeShooting);
        }
        if (weapon == 2)
        {
            GameObject bullet = Instantiate(graneadeprefab);
            bullet.transform.position = bulletInitialTransform.position;
            bullet.GetComponent<Rigidbody2D>().velocity = shootDirection * bulletMaxInitialVelocity * (timeShooting / maxTimeShooting);
        }
        
	}

	// Atualizando a rotaçao da arma e consequentemente da mira baseado em onde o player esta mirando
	// Tb devemos atualizar a escala de bodyTransform para o corpo do nosso Player estar de acordo com a direçao em que o player esta mirando
	void UpdateTargetting(){
		Vector3 mousePosScreen = Input.mousePosition;
		Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
		Vector2 playerToMouse = new Vector2( mousePosWorld.x - transform.position.x,
		                                    mousePosWorld.y - transform.position.y);

		playerToMouse.Normalize();

		float angle = Mathf.Asin(playerToMouse.y)*Mathf.Rad2Deg;
		if( playerToMouse.x < 0f )
			angle = 180-angle;

		if( playerToMouse.x > 0f && bodyTransform.localScale.x > 0f ){
			bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);
		}
		else if( playerToMouse.x < 0f && bodyTransform.localScale.x < 0f ){
			bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);
		}

		gunTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        granadetransform.localEulerAngles = new Vector3(0f, 0f, angle+180);
    }

	// Atualizar a velocidade do nosso Player baseando-se nas teclas pressionadas


	// Funçao chamada em todo frame no qual ha colissao entre o Collider de Player e outro Collider

}
