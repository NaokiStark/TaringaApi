Api de Taringa
=======

## Descripción ##

Api no Oficial de Taringa para .Net 4.5

## Dependencias ##

Newtonsoft.Json
(https://www.nuget.org/packages/Newtonsoft.Json/)

¿Cómo usarla?
-------------

Para poder usarlo, hay que importarlo como referencia e iniciar una nueva instancia con nombre de usuario y contraseña.
Nota, al crear una nueva instancia, no se logueará, hay un método específico para hacerlo.

**Visual Basic**

    Dim Api as TaringaApi
    Api = new TaringaApi("Usuario", "Contraseña")

**C#**

    TaringaApi Api;
    Api = new TaringaApi("Usuario", "Contraseña");

Login de usuario
----------------

Existe un método que inicia sesión y genera un evento de AsyncLogin (onLogin(LoginArgs args))


**Visual Basic**

    Public Function AsyncLogin() as String

**C#**

    public string AsyncLogin()

Cuando se haya concretado la transacción, invoca al evento "onLogin"

LoginArgs contiene:
	string Result: Resultado de la peticion de login

Si es correcto devuelve:

    "true"

Si el usuario y contraseña no coinciden:

    "Datos incorrectos"

Si el usuario se encuentra suspendido

    "Esta cuenta se encuentra suspendida."

Ejemplo:

```C#
	TaringaApi api;
	void Iniciar(string usuario, string pass){
		api= new TaringaApi(usuario, pass);
		api.onLogin+=(args)=>{
			//Hacer algo con el resultado
			Console.WriteLine(args.Result);
		};
		api.AsyncLogin();
	}
	
```

También existe el método sicrónico y devuelve "true" si es exitoso, "Datos incorrectos" y "suspended"

**C#**

    public string SyncLogin()

Métodos Públicos
-------

**string AsyncLogin()**

Inicia sesión

**string sendShout(string shout, int attach_type=0 ,string attach_url="", int privacy=1)**

Envía un shout, los parámetros son los siguientes:

	shout: Cuerpo del shout
	attach_type: Tipo de adjunto (0 por defecto)
	attach_url: Url del adjunto ("" por defecto)
	privacy: Privacidad [0= Todos pueden Comentar, 1= Solo quien sigo puede comentar, 2= Comentarios cerrados](1 por defecto)

Al procesar, si sale correctamente devuelve la url del shout.

Ejemplo:

	api.sendShout("Shout de prueba", 1,"http://k31.kn3.net/taringa/1/3/6/7/5/9/68/naii-/190.gif",2);
	

**string getLastShoutUrl(string id)**

Obtiene el último shout de un usuario específico según su id.

**string getRank(string Nickname)**
**string getRank(int id)**

Obtiene el rango del usuario especificado (ej: "Silver") .

Nickname= El nombre de usuario
id= el id del usuario

## ToDo ##

Implementar Websocket para el RealTime (notificaciones en tiempo real)
Implementar requests asíncronos.

## ue ##

Proximamente voy a subir con más metodos y cositas.

Cualquier pull request es bienvenido.
