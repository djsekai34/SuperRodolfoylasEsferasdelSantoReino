using System.IO;
using UnityEngine;

public class SaveGame 
{
    // Generamos una ruta automatica en el disco duro segun el sistema operativo
    // Se genera de forma automatica en windows en --> C:\Users\tuUsuario\AppData\LocalLow\Afterbit\Super Rodolfo y las Esferas del Santo Reino\RodolfoSG.json
    // En mac npor defecto se generada en --> /Users/tuUsuario/Library/Application Support/Afterbit/Super Rodolfo y las Esferas del Santo Reino/RodolfoSG.json
    private static string RutaArchivo => Path.Combine(Application.persistentDataPath, "RodolfoSG.json");

    // Metodo para guardar
    public static void Guardar(DatosJuego datos)
    {
        // Convertimos el objeto que tenemos en c# en un json, usamos true para que sea legible
        string json = JsonUtility.ToJson(datos, true); 

        // Escribimos el archivo en el disco duro
        File.WriteAllText(RutaArchivo, json);
    }

    // Metodo para cargar
    public static DatosJuego Cargar()
    {
        // Si el archivo existe en nuestro disco duro lo vamos a leer
        if (File.Exists(RutaArchivo))
        {
            string json = File.ReadAllText(RutaArchivo);
            // Lo volvemos a conventir en C#
            return JsonUtility.FromJson<DatosJuego>(json);
        }
        else
        {
            // Si lo habrimos por primera vez y no hay nada le damos un archivo vacio por defecto
            return new DatosJuego();
        }
    }
}
