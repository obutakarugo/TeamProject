﻿ using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScenarioSetter : MonoBehaviour {
	
	
	public enum Route
	{
		Main = 0,
		A = 1,
		B = 2,
		C = 3,
		
		NULL =-1,//Minigameに遷移の予定
		
	}
	
	
	//テキスト一行分に格納されるデータ一覧
	//テキストデータ、※表示時間、次に表示するルート,アニメーション命令
	//追加予定:効果音、CV等の命令
	public struct Scenariodate
	{
		
		public string _text_date;
		public float _time;
		public Route _next_route;
		public CharacterAnimator.Animation _jony_animation;
		public CharacterAnimator.Animation _abery_animation;
		public CharacterAnimator.State _jony_state;
		public CharacterAnimator.State _abery_state;
		public int _camera_number;

		public Scenariodate(string text_date,float time ,Route next_route = Route.Main,
		                    CharacterAnimator.Animation jony_animation = CharacterAnimator.Animation.UpScaling,
		                    CharacterAnimator.Animation abery_animation = CharacterAnimator.Animation.UpScaling,
		                    
		                    CharacterAnimator.State jony_state = CharacterAnimator.State.Normal,
			CharacterAnimator.State abery_state = CharacterAnimator.State.Normal,
			int camera_number = -1)
		{
			this._text_date = text_date;
			this._next_route = next_route;
			this._time = time;

			this._jony_animation = jony_animation;
			this._jony_state = jony_state;
			this._abery_animation = abery_animation;
			this._abery_state = abery_state;
			this._camera_number = camera_number;
		}

	}
	
	
	//メインシナリオ
	
	//テキスト格納用
	List<Scenariodate> _Main = new List<Scenariodate>();
	List<Scenariodate> _A = new List<Scenariodate>();
	List<Scenariodate> _B = new List<Scenariodate>();
	List<Scenariodate> _C = new List<Scenariodate>();
	
	string[] _main_route_text;
	string[] _A_route_text;
	string[] _B_route_text;
	string[] _C_route_text;
	
	Route _next_route;
	string _current_text;
	float _timer;
	int[,] _current_text_number = new int[4,1];
	public int CurrentTextNumber_A{
		get {return _current_text_number[(int)Route.A,0]; }
		set { _current_text_number [(int)Route.A, 0] = value;}
	}
	public int CurrentTextNumber_B{
		get {return _current_text_number[(int)Route.B,0]; }
		set { _current_text_number [(int)Route.B, 0] = value;}
	}
	public int CurrentTextNumber_C{
		get {return _current_text_number[(int)Route.C,0]; }
		set { _current_text_number [(int)Route.C, 0] = value;}
	}

	int _main_text_number = 0;
	int _a_text_number = 0;
	int _b_text_number = 0;
	int _c_text_number = 0;
	
	//フォント用変数
	public GUISkin s_Skin;
	GUIStyle Style;
	GUIStyleState State;
	
	public float[] timer;
	
	int gameroot = 0;

	//他クラス参照類
	CharacterAnimator Jony;
	CharacterAnimator Abery;
	ScenarioText _scenario_text;
	ChangeCamera _view_camera;
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// <summary>
	///
	/// </summary>
	// Use this for initialization
	void Start () {
		Jony = (GameObject.FindGameObjectWithTag ("Jony")).GetComponent<CharacterAnimator>();
		Abery = GameObject.FindGameObjectWithTag ("Abery").GetComponent<CharacterAnimator>();
		_scenario_text = GameObject.FindObjectOfType<ScenarioText> ();
		_view_camera = GameObject.FindObjectOfType<ChangeCamera> ();
		Style = new GUIStyle();
		State = new GUIStyleState();
		
		//CSVデータから、テキストデータ等を読み込む
		var MasterTable = new CSVMasterTable();
		MasterTable.Load();
		foreach (var Master in MasterTable.All)
		{
			
			switch ((Route)Master.CurrentRoute) {
				
			case Route.Main:
				
				_Main.Add (new Scenariodate (Master.Scenario, Master.WatchTime, (Route)Master.NextRoute,
					Master.JonyAnimation, Master.AberyAnimation,
					Master.JonyState, Master.AberyState, Master.CameraNumber));
				
				
				break;
			case Route.A:
				
				_A.Add (new Scenariodate (Master.Scenario, Master.WatchTime, (Route)Master.NextRoute,
					Master.JonyAnimation, Master.AberyAnimation,
					Master.JonyState, Master.AberyState, Master.CameraNumber));
				
				break;
			case Route.B:
				
				_B.Add (new Scenariodate (Master.Scenario, Master.WatchTime, (Route)Master.NextRoute,
					Master.JonyAnimation, Master.AberyAnimation,
					Master.JonyState, Master.AberyState, Master.CameraNumber));
				
				break;
			case Route.C:
				
				_C.Add (new Scenariodate (Master.Scenario, Master.WatchTime, (Route)Master.NextRoute,
					Master.JonyAnimation, Master.AberyAnimation,
					Master.JonyState, Master.AberyState, Master.CameraNumber));
				
				break;
				
			}
			
		}
		
		_A.Add(new Scenariodate("ENDTEXT",0,Route.Main));
		_B.Add(new Scenariodate("ENDTEXT",0,Route.Main));
		_C.Add(new Scenariodate("ENDTEXT",0,Route.Main));
		
		_timer = _Main [0]._time;
		_current_text = _Main [0]._text_date;
		_scenario_text._Text.text = _Main [0]._text_date;
		_next_route = Route.Main;
		
		//StartCoroutine(WaitTimeAndGo());
		
	}
	
	
	// Update is called once per frame
	void Update () {
		
		
		
		if (_timer > 0) {
			
			_timer -= Time.deltaTime;
			
		} else if(_next_route != Route.NULL){
			
			//テキストデータを更新	
			UpdateScenerio (_next_route);
	

		}
		//ミニゲーム等で、テキスト表示を一旦中止.
		//デバッグキーがなくなったら、スイッチ分の中に入れるべき
		if (_next_route == Route.NULL && _timer <= 0) {
			
		
			_current_text = "";
			//Fix: α版用デバッグキー　右クリックで、メインシナリオに遷移
			if (Input.GetMouseButtonDown (1)) {
				_next_route = Route.Main;
				UpdateScenerio (_next_route);
			}
			
			if (Input.GetKeyDown(KeyCode.A)) {
				_next_route = Route.A;
				UpdateScenerio (_next_route);
				
				//シナリオをスキップ//次の分岐のテキストまで配列番号を更新
				while (true) {
					if (_B [CurrentTextNumber_B]._next_route != Route.B) {

						CurrentTextNumber_B++;
						break;
					}
					CurrentTextNumber_B++;
				};
				while (true) {
					if (_C [CurrentTextNumber_C]._next_route != Route.C) {

						CurrentTextNumber_C++;
						break;
					}
					CurrentTextNumber_C++;
				};

				
			}

			if (Input.GetKeyDown(KeyCode.B)) {
				_next_route = Route.B;
				UpdateScenerio (_next_route);
				//シナリオをスキップ//次の分岐のテキストまで配列番号を更新
				while (true) {
					if (_A [CurrentTextNumber_A]._next_route != Route.A) {

						CurrentTextNumber_A++;
						break;
					}
					CurrentTextNumber_A++;
				};
				while (true) {
					if (_C [CurrentTextNumber_C]._next_route != Route.C) {

						CurrentTextNumber_C++;
						break;
					}
					CurrentTextNumber_C++;
				};
			}

			if (Input.GetKeyDown(KeyCode.C)) {
				_next_route = Route.C;
				UpdateScenerio (_next_route);

				//シナリオをスキップ//次の分岐のテキストまで配列番号を更新
				while (true) {
					if (_A [CurrentTextNumber_A]._next_route != Route.A) {

						CurrentTextNumber_A++;
						break;
					}
					CurrentTextNumber_A++;
				};
				while (true) {
					if (_B [CurrentTextNumber_B]._next_route != Route.B) {

						CurrentTextNumber_B++;
						break;
					}
					CurrentTextNumber_B++;
				};
			}
			

		}
		
		
		
	}
	
	
	void UpdateScenerio(Route route)
	{

		Scenariodate data = new Scenariodate();
		switch (route) {

		case Route.Main:
			 
			/*	_current_text = _Main [_current_text_number [(int)route, 0]]._text_date;
			_timer = _Main [_current_text_number [(int)route, 0]]._time;
			_next_route = _Main [_current_text_number [(int)route, 0]]._next_route;
			Jony._current_animation = _Main [_current_text_number [(int)route, 0]]._jony_animation;
			Jony._current_state  =  _Main [_current_text_number [(int)route, 0]]._jony_state;
			Abery._current_animation = _Main [_current_text_number [(int)route, 0]]._abery_animation;
			Abery._current_state = _Main [_current_text_number [(int)route, 0]]._abery_state;
*/
			data = _Main [_current_text_number [(int)Route.Main, 0]];
			_current_text_number [(int)Route.Main, 0]++;
			break;
		case Route.A:
			/*	_current_text = _A [_current_text_number [(int)route, 0]]._text_date;
			_timer = _A [_current_text_number [(int)route, 0]]._time;
			_next_route = _A [_current_text_number [(int)route, 0]]._next_route;
			Jony._current_animation = _Main [_current_text_number [(int)route, 0]]._jony_animation;
			Jony._current_state  =  _Main [_current_text_number [(int)route, 0]]._jony_state;
			Abery._current_animation = _Main [_current_text_number [(int)route, 0]]._abery_animation;
			Abery._current_state = _Main [_current_text_number [(int)route, 0]]._abery_state;
*/
			data = _A [_current_text_number [(int)route, 0]];
			_current_text_number [(int)Route.A, 0]++;
			break;
		case Route.B:
			/*_current_text = _B [_current_text_number [(int)route, 0]]._text_date;
			_timer        = _B [_current_text_number [(int)route, 0]]._time;
			_next_route   = _B [_current_text_number [(int)route, 0]]._next_route;
			Jony._current_animation = _Main [_current_text_number [(int)route, 0]]._jony_animation;
			Jony._current_state  =  _Main [_current_text_number [(int)route, 0]]._jony_state;
			Abery._current_animation = _Main [_current_text_number [(int)route, 0]]._abery_animation;
			Abery._current_state = _Main [_current_text_number [(int)route, 0]]._abery_state;
*/
			data = _B [_current_text_number [(int)route, 0]];
			_current_text_number [(int)Route.B, 0]++;
			break;
		case Route.C:
			/*	_current_text = _C [_current_text_number [(int)route, 0]]._text_date;
			_timer        = _C [_current_text_number [(int)route, 0]]._time;
			_next_route   = _C [_current_text_number [(int)route, 0]]._next_route;
			Jony._current_animation = _Main [_current_text_number [(int)route, 0]]._jony_animation;
			Jony._current_state  =  _Main [_current_text_number [(int)route, 0]]._jony_state;
			Abery._current_animation = _Main [_current_text_number [(int)route, 0]]._abery_animation;
			Abery._current_state = _Main [_current_text_number [(int)route, 0]]._abery_state;
*/
			data = _C [_current_text_number [(int)route, 0]];
			_current_text_number [(int)Route.C, 0]++;
			break;
		case Route.NULL:
			break;
		}

		if (route != Route.NULL) {
			_current_text = data._text_date;
			_scenario_text._Text.text = data._text_date;
			_timer = data._time;
			_next_route = data._next_route;
			//(CharacterAnimator.Animation) のキャストはいらなくなるかも
			Abery._current_animation = (CharacterAnimator.Animation)data._abery_animation;
			Abery._next_state = (CharacterAnimator.State)data._abery_state;
			Jony._current_animation = (CharacterAnimator.Animation)data._jony_animation;
			Jony._next_state = (CharacterAnimator.State)data._jony_state;
			_view_camera._SetNumber = data._camera_number;
		}
	}
	
	IEnumerator WaitTimeAndGo()
	{
		for (int i = 0; i < timer.Length;i++ )
		{
			Debug.Log(timer);
			yield return new WaitForSeconds(timer[i]);
			//text_number++;
		}
	}
	
	void OnGUI()
	{
		
		/*	GUI.skin = s_Skin;
		
		Style.fontSize = Screen.height/22;
		Style.normal = State;
		
		State.textColor = Color.black;
		
		GUI.Label(new Rect(Screen.width/25, Screen.height / 2 + Screen.height / 4, 0, Screen.height / 10), _current_text, Style);
*/
	}
}
