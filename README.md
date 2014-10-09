Api de Taringa
=======

## Descripción ##

Api no Oficial de Taringa para .Net

¿Cómo usarla?
-------------

Para poder usarlo, hay que importarlo como referencia e iniciar una nueva instancia con nombre de usuario y contraseña.
Nota, al crear una nueva instancia, no se logueará, hay un método específico para hacerlo.

**Visual Basic**

    Dim Api as ClassLibrary1.TaringaApi
    Api = new ClassLibrary1.TaringaApi("Usuario", "Contraseña")

**C#**

    ClassLibrary1.TaringaApi Api;
    Api = new ClassLibrary1.TaringaApi("Usuario", "Contraseña");

Login de usuario
----------------

Existe un método que inicia sesión y devuelve el resultado en json, el método es el siguente:

**Visual Basic**

    Public Function login() as String

**C#**

    public string login()

Si es correcto devuelve en formato json lo siguente:

    {"status":1,"data":""}

Si el usuario y contraseña no coinciden:

    "{\"status\":0,\"data\":\"Datos no v&aacute;lidos<br\\/><a href=\\\"\\/recuperar-clave\\\">&iquest;Olvidaste tu contrase&ntilde;a?<\\/a>\"}"

Si el usuario se encuentra suspendido

    "{\"status\":2,\"data\":{\"causa\":\"[causa]\",\"fecha\":\"[fecha]\",\"duracion\":\"[duración]\",\"fecha_rehabilitacion\":\"[rehabilitación]\",\"falta\":\"[Tiempo restante]\"}}"

Ejemplo:

**Visual Basic**

    Public sub Main()
    
    Dim Api as ClassLibrary1.TaringaApi
    Console.Write("Escriba el usuario: ")
    Dim Usuario as String = Console.ReadLine()
    Console.Write("Escriba su contraseña: ")
    Dim Password as String = Console.ReadLine()
    Console.WriteLine("Procesando")
    
    Api= New ClassLibrary1.TaringaApi(Usuario, Password)
    
    Dim Resultado as String = Api.login()
    Console.WriteLine("Resultado: {0}", Resultado)
    Console.Read()     
      
    End Sub

**C#**

    static void Main(string[] args){
	    Console.WriteLine("Escriba el Usuario: ");
	    var usuario = Console.ReadLine();
	    Console.WriteLine("Escriba su contraseña: ");
	    var password = Console.ReadLine();
	    Console.WriteLine("Procesando");
	    
	    var api = new ClassLibrary1.TaringaApi(usuario, password);
	    var resultado = api.login();
	    Console.WriteLine("Resultado: {0}", resultado);
	    Console.Read();
    }

Métodos Públicos
-------

**string login()**

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

## we ##

Proximamente voy a subir con más metodos y cositas.

Cualquier pull request es bienvenido.