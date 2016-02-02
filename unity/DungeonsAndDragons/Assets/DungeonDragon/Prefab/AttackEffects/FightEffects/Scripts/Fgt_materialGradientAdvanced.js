var Gradient : Gradient;
var targetMaterialSlot:int=0;
var timeMultiplier:float=1;
var TintColor:boolean=true;
var MainColor:boolean=false;
var loop:boolean=false;

var resetTimeOnActivate:boolean=false;

private var curColor:Color;
private var time:float=0.0f;



function Start ()
{

if (TintColor==true) GetComponent.<Renderer>().materials[targetMaterialSlot].SetColor ("_TintColor", Color(0, 0, 0, 0));
if (MainColor==true) GetComponent.<Renderer>().materials[targetMaterialSlot].SetColor ("_Color", Color(0, 0, 0, 0));

}

function OnEnable () {
if (resetTimeOnActivate==true) time=0;

}


function Update () {
time+=Time.deltaTime*timeMultiplier;
curColor=Gradient.Evaluate(time);

if (TintColor==true) GetComponent.<Renderer>().materials[targetMaterialSlot].SetColor ("_TintColor", curColor);
if (MainColor==true) GetComponent.<Renderer>().materials[targetMaterialSlot].SetColor ("_Color", curColor);

if ((loop==true) && (time>=1.0f)) time-=1.0f;
}

