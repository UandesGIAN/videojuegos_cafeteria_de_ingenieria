using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class QuestionGeneratorEditor : EditorWindow
{
    private int questionsPerCategory = 20;
    
    [MenuItem("Tools/Question Generator")]
    public static void ShowWindow()
    {
        GetWindow<QuestionGeneratorEditor>("Question Generator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Generador de Preguntas Automático", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        questionsPerCategory = EditorGUILayout.IntField("Preguntas por Categoría", questionsPerCategory);
        
        GUILayout.Space(5);
        EditorGUILayout.HelpBox(
            $"Se generarán {questionsPerCategory} preguntas para cada una de las 6 categorías.\n" +
            $"Total: {questionsPerCategory * 6} preguntas\n\n" +
            $"El QuestionPool se detectará automáticamente.", 
            MessageType.Info
        );
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Generar Preguntas Automáticamente", GUILayout.Height(40)))
        {
            GenerateQuestions();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Limpiar Question Pool"))
        {
            ClearQuestionPool();
        }
    }
    
    private void GenerateQuestions()
    {
        // Buscar QuestionPool automáticamente
        string[] guids = AssetDatabase.FindAssets("t:QuestionPool");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Error", 
                "No se encontró ningún QuestionPool.\n\nCrea uno primero usando:\nClick derecho → Create → Battle → Question Pool", 
                "OK"
            );
            return;
        }
        
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        QuestionPool targetQuestionPool = AssetDatabase.LoadAssetAtPath<QuestionPool>(path);
        
        if (targetQuestionPool == null)
        {
            EditorUtility.DisplayDialog("Error", "No se pudo cargar el QuestionPool.", "OK");
            return;
        }
        
        Debug.Log($"QuestionPool encontrado: {path}");
        
        // Limpiar arrays existentes
        targetQuestionPool.civilQuestions = new Question[questionsPerCategory];
        targetQuestionPool.computacionQuestions = new Question[questionsPerCategory];
        targetQuestionPool.ambientalQuestions = new Question[questionsPerCategory];
        targetQuestionPool.obrasQuestions = new Question[questionsPerCategory];
        targetQuestionPool.electricaQuestions = new Question[questionsPerCategory];
        targetQuestionPool.planComunQuestions = new Question[questionsPerCategory];
        
        // Generar preguntas para cada categoría
        GenerateCategoryQuestions("Civil", targetQuestionPool.civilQuestions);
        GenerateCategoryQuestions("Computacion", targetQuestionPool.computacionQuestions);
        GenerateCategoryQuestions("Ambiental", targetQuestionPool.ambientalQuestions);
        GenerateCategoryQuestions("Obras", targetQuestionPool.obrasQuestions);
        GenerateCategoryQuestions("Electrica", targetQuestionPool.electricaQuestions);
        GenerateCategoryQuestions("PlanComun", targetQuestionPool.planComunQuestions);
        
        // Combinar todas en allQuestions
        CombineAllQuestions(targetQuestionPool);
        
        EditorUtility.SetDirty(targetQuestionPool);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog(
            "¡Éxito!", 
            $"Se generaron {questionsPerCategory * 6} preguntas correctamente.\n\n" +
            $"✓ Civil: {questionsPerCategory}\n" +
            $"✓ Computación: {questionsPerCategory}\n" +
            $"✓ Ambiental: {questionsPerCategory}\n" +
            $"✓ Obras: {questionsPerCategory}\n" +
            $"✓ Eléctrica: {questionsPerCategory}\n" +
            $"✓ Plan Común: {questionsPerCategory}\n\n" +
            $"QuestionPool actualizado: {targetQuestionPool.name}", 
            "OK"
        );
        
        // Seleccionar el QuestionPool en el Project
        Selection.activeObject = targetQuestionPool;
        EditorGUIUtility.PingObject(targetQuestionPool);
    }
    
    private void GenerateCategoryQuestions(string category, Question[] questionArray)
    {
        for (int i = 0; i < questionsPerCategory; i++)
        {
            Question newQuestion = new Question();
            ConfigureQuestion(newQuestion, category, i + 1);
            questionArray[i] = newQuestion;
        }
    }
    
    private void ConfigureQuestion(Question question, string category, int number)
    {
        question.category = category;
        
        switch (category)
        {
            case "Civil":
                ConfigureCivilQuestion(question, number);
                break;
            case "Computacion":
                ConfigureComputacionQuestion(question, number);
                break;
            case "Ambiental":
                ConfigureAmbientalQuestion(question, number);
                break;
            case "Obras":
                ConfigureObrasQuestion(question, number);
                break;
            case "Electrica":
                ConfigureElectricaQuestion(question, number);
                break;
            case "PlanComun":
                ConfigurePlanComunQuestion(question, number);
                break;
        }
    }
    
    private void ConfigureCivilQuestion(Question q, int num)
    {
        string[] questions = new string[]
        {
            "¿Cuál es el módulo de elasticidad del acero?",
            "¿Qué tipo de carga es el peso propio de una estructura?",
            "¿Cuál es la resistencia característica del hormigón H30?",
            "¿Qué elemento estructural trabaja principalmente a flexión?",
            "¿Cuál es la función principal de una viga?",
            "¿Qué tipo de suelo es más adecuado para cimentaciones?",
            "¿Cuál es el esfuerzo que soporta una columna?",
            "¿Qué es el momento flector?",
            "¿Cuál es la diferencia entre una viga simplemente apoyada y una empotrada?",
            "¿Qué es la fatiga en materiales?",
            "¿Cuál es el factor de seguridad típico en estructuras de acero?",
            "¿Qué es un diagrama de corte y momento?",
            "¿Cuál es la función de los estribos en una viga de hormigón?",
            "¿Qué es la fluencia del hormigón?",
            "¿Cuál es el módulo de Poisson del acero aproximadamente?",
            "¿Qué es una carga puntual?",
            "¿Cuál es la diferencia entre tensión y compresión?",
            "¿Qué es el pandeo en columnas?",
            "¿Cuál es la unidad de medida del momento flector?",
            "¿Qué es una estructura isostática?"
        };
        
        string[][] answers = new string[][]
        {
            new[] { "200 GPa", "100 GPa", "300 GPa", "150 GPa" },
            new[] { "Carga muerta", "Carga viva", "Carga dinámica", "Carga de viento" },
            new[] { "30 MPa", "20 MPa", "40 MPa", "25 MPa" },
            new[] { "Viga", "Columna", "Muro", "Zapata" },
            new[] { "Soportar cargas transversales", "Soportar cargas axiales", "Transmitir momentos", "Resistir torsión" },
            new[] { "Suelo rocoso", "Suelo arcilloso", "Suelo orgánico", "Suelo arenoso suelto" },
            new[] { "Compresión", "Tensión", "Flexión", "Torsión" },
            new[] { "Momento producido por fuerzas que causan flexión", "Fuerza cortante", "Esfuerzo axial", "Carga puntual" },
            new[] { "La empotrada no puede rotar en los apoyos", "La empotrada es más corta", "No hay diferencia", "La apoyada es más resistente" },
            new[] { "Daño por cargas cíclicas", "Deformación plástica", "Fractura frágil", "Corrosión" },
            new[] { "2.0 a 3.0", "1.0 a 1.5", "4.0 a 5.0", "0.5 a 1.0" },
            new[] { "Representación gráfica de esfuerzos internos", "Diagrama de cargas", "Mapa de tensiones", "Gráfico de deformaciones" },
            new[] { "Resistir esfuerzo cortante", "Aumentar la flexión", "Decorar", "Reducir peso" },
            new[] { "Deformación a largo plazo bajo carga constante", "Fractura inmediata", "Aumento de resistencia", "Cambio de color" },
            new[] { "0.3", "0.5", "0.1", "0.7" },
            new[] { "Carga aplicada en un punto específico", "Carga distribuida", "Carga en toda la superficie", "Carga de viento" },
            new[] { "Tensión estira, compresión comprime", "Son iguales", "Tensión comprime", "No hay diferencia" },
            new[] { "Inestabilidad lateral por compresión", "Rotura por tensión", "Deformación elástica", "Corrosión" },
            new[] { "Newton-metro (N·m)", "Newton (N)", "Pascal (Pa)", "Kilogramo (kg)" },
            new[] { "Sistema estáticamente determinado", "Sistema hiperstático", "Sistema inestable", "Sistema dinámico" }
        };
        
        int[] correctAnswers = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int index = (num - 1) % questions.Length;
        q.questionText = questions[index];
        q.answers = answers[index];
        q.correctAnswerIndex = correctAnswers[index];
    }
    
    private void ConfigureComputacionQuestion(Question q, int num)
    {
        string[] questions = new string[]
        {
            "¿Qué estructura de datos usa el principio LIFO?",
            "¿Cuál es la complejidad temporal de la búsqueda binaria?",
            "¿Qué significa SQL?",
            "¿Cuál es el protocolo de transferencia de hipertexto?",
            "¿Qué paradigma usa Python principalmente?",
            "¿Cuál es la diferencia entre GET y POST?",
            "¿Qué es un algoritmo recursivo?",
            "¿Cuál es la complejidad del algoritmo QuickSort en promedio?",
            "¿Qué es una variable global?",
            "¿Cuál es el resultado de 5 // 2 en Python?",
            "¿Qué es un array?",
            "¿Cuál es la función de un compilador?",
            "¿Qué es Git?",
            "¿Cuál es el puerto por defecto de HTTP?",
            "¿Qué es una API?",
            "¿Cuál es la diferencia entre Java y JavaScript?",
            "¿Qué es una base de datos relacional?",
            "¿Cuál es el resultado de '2' + 2 en JavaScript?",
            "¿Qué es el Big O notation?",
            "¿Cuál es la función de un IDE?"
        };
        
        string[][] answers = new string[][]
        {
            new[] { "Pila (Stack)", "Cola (Queue)", "Árbol", "Grafo" },
            new[] { "O(log n)", "O(n)", "O(n²)", "O(1)" },
            new[] { "Structured Query Language", "Simple Question Language", "System Quality Language", "Standard Query List" },
            new[] { "HTTP", "FTP", "SMTP", "TCP" },
            new[] { "Multiparadigma", "Solo POO", "Solo funcional", "Solo procedural" },
            new[] { "GET obtiene datos, POST envía datos", "Son idénticos", "GET es más seguro", "POST es más rápido" },
            new[] { "Un algoritmo que se llama a sí mismo", "Un algoritmo iterativo", "Un algoritmo sin fin", "Un algoritmo lineal" },
            new[] { "O(n log n)", "O(n²)", "O(n)", "O(log n)" },
            new[] { "Variable accesible desde cualquier parte del programa", "Variable local", "Variable constante", "Variable temporal" },
            new[] { "2", "2.5", "Error", "0" },
            new[] { "Estructura de datos con elementos del mismo tipo", "Función matemática", "Variable simple", "Algoritmo" },
            new[] { "Traducir código fuente a código máquina", "Ejecutar código", "Depurar errores", "Guardar archivos" },
            new[] { "Sistema de control de versiones", "Lenguaje de programación", "Base de datos", "Framework web" },
            new[] { "80", "443", "8080", "3000" },
            new[] { "Interfaz de Programación de Aplicaciones", "Algoritmo de programación", "Aplicación web", "Protocolo de red" },
            new[] { "Son lenguajes diferentes para distintos propósitos", "Son el mismo lenguaje", "Java es versión nueva", "JavaScript es más antiguo" },
            new[] { "Base de datos con tablas relacionadas", "Base de datos no estructurada", "Base de datos en memoria", "Base de datos de grafos" },
            new[] { "'22' (string)", "4", "Error", "undefined" },
            new[] { "Notación de complejidad algorítmica", "Tipo de variable", "Lenguaje de programación", "Sistema operativo" },
            new[] { "Entorno de desarrollo integrado", "Sistema operativo", "Base de datos", "Servidor web" }
        };
        
        int[] correctAnswers = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int index = (num - 1) % questions.Length;
        q.questionText = questions[index];
        q.answers = answers[index];
        q.correctAnswerIndex = correctAnswers[index];
    }
    
    private void ConfigureAmbientalQuestion(Question q, int num)
    {
        string[] questions = new string[]
        {
            "¿Qué gas es el principal responsable del efecto invernadero?",
            "¿Cuál es el tratamiento primario en una planta de aguas residuales?",
            "¿Qué significa DBO?",
            "¿Cuál es la capa de la atmósfera que contiene el ozono?",
            "¿Qué es la eutrofización?",
            "¿Cuál es el pH neutral del agua?",
            "¿Qué es un bioindicador?",
            "¿Cuál es la principal fuente de contaminación del agua?",
            "¿Qué es la huella de carbono?",
            "¿Cuál es el objetivo del desarrollo sostenible?",
            "¿Qué es la biodegradabilidad?",
            "¿Cuál es la función de un humedal?",
            "¿Qué es el compostaje?",
            "¿Cuál es la diferencia entre reciclaje y reutilización?",
            "¿Qué es la lluvia ácida?",
            "¿Cuál es el protocolo internacional sobre cambio climático?",
            "¿Qué es un ecosistema?",
            "¿Cuál es la importancia de la capa de ozono?",
            "¿Qué es la contaminación acústica?",
            "¿Cuál es la energía renovable más utilizada?"
        };
        
        string[][] answers = new string[][]
        {
            new[] { "CO₂ (Dióxido de carbono)", "O₂ (Oxígeno)", "N₂ (Nitrógeno)", "H₂ (Hidrógeno)" },
            new[] { "Sedimentación", "Desinfección", "Filtración avanzada", "Oxidación química" },
            new[] { "Demanda Bioquímica de Oxígeno", "Densidad Biológica Orgánica", "Degradación Bacterial Óptima", "Depuración de Biomas" },
            new[] { "Estratosfera", "Troposfera", "Mesosfera", "Termosfera" },
            new[] { "Exceso de nutrientes en agua", "Falta de oxígeno", "Contaminación por metales", "Erosión del suelo" },
            new[] { "7", "5", "9", "1" },
            new[] { "Organismo que indica calidad ambiental", "Tipo de bacteria", "Instrumento de medición", "Químico degradable" },
            new[] { "Actividades industriales y urbanas", "Lluvia", "Evaporación", "Fotosíntesis" },
            new[] { "Cantidad total de GEI emitidos", "Marca en el suelo", "Tipo de contaminación", "Medida de temperatura" },
            new[] { "Satisfacer necesidades sin comprometer futuras generaciones", "Crecer económicamente", "Aumentar población", "Desarrollar tecnología" },
            new[] { "Capacidad de descomponerse naturalmente", "Resistencia a bacterias", "Toxicidad", "Durabilidad" },
            new[] { "Filtrar agua y proporcionar hábitat", "Generar energía", "Producir alimentos", "Almacenar gases" },
            new[] { "Descomposición de materia orgánica", "Quema de residuos", "Reciclaje de plástico", "Filtración de agua" },
            new[] { "Reciclaje transforma, reutilización mantiene uso", "Son lo mismo", "Reciclaje es mejor", "Reutilización es industrial" },
            new[] { "Precipitación con contaminantes ácidos", "Lluvia muy fuerte", "Lluvia tropical", "Lluvia con minerales" },
            new[] { "Acuerdo de París", "Protocolo HTTP", "Tratado de Versalles", "Acuerdo NAFTA" },
            new[] { "Sistema de organismos y su ambiente", "Tipo de bioma", "Clima específico", "Región geográfica" },
            new[] { "Protege de radiación UV", "Produce oxígeno", "Genera lluvia", "Controla temperatura" },
            new[] { "Exceso de ruido dañino", "Falta de sonido", "Música alta", "Vibraciones del suelo" },
            new[] { "Hidroeléctrica", "Nuclear", "Carbón", "Gas natural" }
        };
        
        int[] correctAnswers = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int index = (num - 1) % questions.Length;
        q.questionText = questions[index];
        q.answers = answers[index];
        q.correctAnswerIndex = correctAnswers[index];
    }
    
    private void ConfigureObrasQuestion(Question q, int num)
    {
        string[] questions = new string[]
        {
            "¿Qué es un diagrama de Gantt?",
            "¿Cuál es la ruta crítica en un proyecto?",
            "¿Qué significa CPM?",
            "¿Cuál es la función de un contratista general?",
            "¿Qué es el presupuesto de obra?",
            "¿Cuál es la diferencia entre obra gruesa y terminaciones?",
            "¿Qué es un cronograma de obra?",
            "¿Cuál es el objetivo de la planificación de obra?",
            "¿Qué es el control de calidad en obra?",
            "¿Cuál es la función de un inspector técnico de obra?",
            "¿Qué es una partida presupuestaria?",
            "¿Cuál es la importancia del plan de seguridad?",
            "¿Qué es el libro de obra?",
            "¿Cuál es la diferencia entre subcontratista y proveedor?",
            "¿Qué es el flujo de caja en un proyecto?",
            "¿Cuál es la función del ingeniero residente?",
            "¿Qué es una carta Gantt?",
            "¿Cuál es el objetivo de la gestión de riesgos?",
            "¿Qué es el avance de obra?",
            "¿Cuál es la importancia de las especificaciones técnicas?"
        };
        
        string[][] answers = new string[][]
        {
            new[] { "Herramienta de programación temporal", "Tipo de contrato", "Documento legal", "Plano estructural" },
            new[] { "Secuencia más larga de actividades", "Actividad más cara", "Primera actividad", "Última tarea" },
            new[] { "Critical Path Method", "Construction Planning Model", "Cost Production Management", "Contract Process Manual" },
            new[] { "Coordinar y ejecutar la obra completa", "Solo proveer materiales", "Diseñar el proyecto", "Supervisar contabilidad" },
            new[] { "Estimación de costos del proyecto", "Plan de trabajo", "Lista de trabajadores", "Cronograma" },
            new[] { "Gruesa es estructura, terminaciones son acabados", "Son lo mismo", "Gruesa es más cara", "Terminaciones van primero" },
            new[] { "Programación temporal de actividades", "Lista de materiales", "Presupuesto detallado", "Plano de obra" },
            new[] { "Optimizar recursos y tiempo", "Aumentar costos", "Contratar personal", "Comprar materiales" },
            new[] { "Verificación de cumplimiento de estándares", "Contabilidad de costos", "Programación de tareas", "Diseño estructural" },
            new[] { "Supervisar cumplimiento de planos y normas", "Ejecutar la obra", "Diseñar", "Pagar a trabajadores" },
            new[] { "Ítem específico del presupuesto", "Documento legal", "Tipo de material", "Herramienta de construcción" },
            new[] { "Prevenir accidentes y proteger trabajadores", "Aumentar velocidad", "Reducir costos", "Mejorar calidad" },
            new[] { "Registro oficial de la obra", "Libro de contabilidad", "Manual de procedimientos", "Catálogo de materiales" },
            new[] { "Subcontratista ejecuta trabajos, proveedor suministra", "Son lo mismo", "Proveedor ejecuta", "Subcontratista solo vende" },
            new[] { "Entradas y salidas de dinero", "Diagrama de red", "Ruta crítica", "Lista de tareas" },
            new[] { "Dirigir ejecución diaria de la obra", "Diseñar el proyecto", "Aprobar pagos", "Vender materiales" },
            new[] { "Diagrama de barras para programación", "Tipo de contrato", "Documento legal", "Plano arquitectónico" },
            new[] { "Identificar y mitigar problemas potenciales", "Aumentar riesgos", "Eliminar seguros", "Acelerar obra" },
            new[] { "Porcentaje de trabajo completado", "Cantidad de trabajadores", "Costo total", "Tiempo transcurrido" },
            new[] { "Definen requisitos técnicos detallados", "Son opcionales", "Solo para presupuesto", "Reemplazan planos" }
        };
        
        int[] correctAnswers = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int index = (num - 1) % questions.Length;
        q.questionText = questions[index];
        q.answers = answers[index];
        q.correctAnswerIndex = correctAnswers[index];
    }
    
    private void ConfigureElectricaQuestion(Question q, int num)
    {
        string[] questions = new string[]
        {
            "¿Cuál es la ley de Ohm?",
            "¿Qué es la corriente alterna (AC)?",
            "¿Cuál es la unidad de resistencia eléctrica?",
            "¿Qué es un transformador?",
            "¿Cuál es la diferencia entre voltaje y corriente?",
            "¿Qué es la potencia eléctrica?",
            "¿Cuál es la frecuencia de la red eléctrica en Chile?",
            "¿Qué es un cortocircuito?",
            "¿Cuál es la función de un fusible?",
            "¿Qué es la puesta a tierra?",
            "¿Cuál es la ley de Kirchhoff de corrientes?",
            "¿Qué es un capacitor?",
            "¿Cuál es la unidad de capacitancia?",
            "¿Qué es un inductor?",
            "¿Cuál es la diferencia entre AC y DC?",
            "¿Qué es la impedancia?",
            "¿Cuál es el código de colores de resistencias?",
            "¿Qué es un diodo?",
            "¿Cuál es la función de un interruptor diferencial?",
            "¿Qué es la potencia aparente?"
        };
        
        string[][] answers = new string[][]
        {
            new[] { "V = I × R", "P = V × I", "E = m × c²", "F = m × a" },
            new[] { "Corriente que cambia de dirección periódicamente", "Corriente constante", "Corriente negativa", "Corriente sin voltaje" },
            new[] { "Ohmio (Ω)", "Voltio (V)", "Amperio (A)", "Watt (W)" },
            new[] { "Dispositivo que cambia niveles de voltaje", "Generador de corriente", "Medidor eléctrico", "Interruptor automático" },
            new[] { "Voltaje es diferencia de potencial, corriente es flujo de carga", "Son lo mismo", "Voltaje es más peligroso", "Corriente es estática" },
            new[] { "P = V × I", "P = I × R", "P = V / R", "P = I + V" },
            new[] { "50 Hz", "60 Hz", "100 Hz", "120 Hz" },
            new[] { "Conexión directa entre fase y neutro/tierra", "Interrupción del circuito", "Exceso de voltaje", "Falta de corriente" },
            new[] { "Proteger contra sobrecorriente", "Generar voltaje", "Medir corriente", "Almacenar energía" },
            new[] { "Conexión de seguridad a tierra", "Cable de fase", "Aislante eléctrico", "Generador de voltaje" },
            new[] { "La suma de corrientes entrantes = suma de salientes", "V = I × R", "La corriente es constante", "El voltaje se conserva" },
            new[] { "Componente que almacena carga eléctrica", "Generador de corriente", "Resistor variable", "Interruptor" },
            new[] { "Faradio (F)", "Ohmio (Ω)", "Henrio (H)", "Voltio (V)" },
            new[] { "Componente que almacena energía en campo magnético", "Capacitor", "Resistor", "Diodo" },
            new[] { "AC cambia dirección, DC fluye en una dirección", "Son lo mismo", "AC es más peligrosa", "DC es más moderna" },
            new[] { "Oposición total al flujo de corriente AC", "Solo resistencia", "Solo capacitancia", "Voltaje total" },
            new[] { "Sistema de bandas de colores para identificar valor", "Medidor de temperatura", "Indicador de voltaje", "Sistema de seguridad" },
            new[] { "Componente que permite corriente en una dirección", "Resistor variable", "Generador", "Interruptor" },
            new[] { "Detectar fugas de corriente a tierra", "Medir voltaje", "Generar corriente", "Regular temperatura" },
            new[] { "Producto de voltaje y corriente (VA)", "Solo potencia activa", "Potencia perdida", "Voltaje máximo" }
        };
        
        int[] correctAnswers = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int index = (num - 1) % questions.Length;
        q.questionText = questions[index];
        q.answers = answers[index];
        q.correctAnswerIndex = correctAnswers[index];
    }
    
    private void ConfigurePlanComunQuestion(Question q, int num)
    {
        string[] questions = new string[]
        {
            "¿Cuál es la derivada de x²?",
            "¿Cuánto es la raíz cuadrada de 144?",
            "¿Cuál es la fórmula del área de un círculo?",
            "¿Qué es la segunda ley de Newton?",
            "¿Cuál es la velocidad de la luz en el vacío?",
            "¿Qué es un vector?",
            "¿Cuál es la unidad de fuerza en el SI?",
            "¿Qué es la integral?",
            "¿Cuánto es sen(90°)?",
            "¿Cuál es la fórmula de la energía cinética?",
            "¿Qué es un número primo?",
            "¿Cuál es el teorema de Pitágoras?",
            "¿Qué es la aceleración?",
            "¿Cuánto es log₁₀(100)?",
            "¿Cuál es la primera ley de la termodinámica?",
            "¿Qué es una función lineal?",
            "¿Cuál es la fórmula de la velocidad?",
            "¿Qué es un ángulo recto?",
            "¿Cuánto es la suma de ángulos internos de un triángulo?",
            "¿Qué es la gravedad en la Tierra aproximadamente?"
        };
        
        string[][] answers = new string[][]
        {
            new[] { "2x", "x", "x²", "2" },
            new[] { "12", "11", "13", "14" },
            new[] { "πr²", "2πr", "πd", "r²" },
            new[] { "F = ma", "E = mc²", "V = IR", "PV = nRT" },
            new[] { "300,000 km/s", "150,000 km/s", "450,000 km/s", "200,000 km/s" },
            new[] { "Magnitud con dirección", "Solo magnitud", "Unidad de medida", "Tipo de fuerza" },
            new[] { "Newton (N)", "Joule (J)", "Watt (W)", "Pascal (Pa)" },
            new[] { "Antiderivada de una función", "Derivada", "Multiplicación", "División" },
            new[] { "1", "0", "-1", "0.5" },
            new[] { "½mv²", "mgh", "mv", "ma" },
            new[] { "Número divisible solo por 1 y sí mismo", "Número par", "Número impar", "Número negativo" },
            new[] { "a² + b² = c²", "a + b = c", "ab = c", "a/b = c" },
            new[] { "Cambio de velocidad en el tiempo", "Velocidad constante", "Distancia recorrida", "Fuerza aplicada" },
            new[] { "2", "1", "10", "100" },
            new[] { "La energía se conserva", "La energía se crea", "La energía se destruye", "No hay energía" },
            new[] { "f(x) = mx + b", "f(x) = x²", "f(x) = 1/x", "f(x) = eˣ" },
            new[] { "v = d/t", "v = at", "v = ft", "v = d·t" },
            new[] { "90 grados", "45 grados", "180 grados", "60 grados" },
            new[] { "180°", "360°", "90°", "270°" },
            new[] { "9.8 m/s²", "10 m/s²", "8 m/s²", "12 m/s²" }
        };
        
        int[] correctAnswers = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int index = (num - 1) % questions.Length;
        q.questionText = questions[index];
        q.answers = answers[index];
        q.correctAnswerIndex = correctAnswers[index];
    }
    
    private void CombineAllQuestions(QuestionPool targetQuestionPool)
    {
        List<Question> allQuestionsList = new List<Question>();
        
        allQuestionsList.AddRange(targetQuestionPool.civilQuestions);
        allQuestionsList.AddRange(targetQuestionPool.computacionQuestions);
        allQuestionsList.AddRange(targetQuestionPool.ambientalQuestions);
        allQuestionsList.AddRange(targetQuestionPool.obrasQuestions);
        allQuestionsList.AddRange(targetQuestionPool.electricaQuestions);
        allQuestionsList.AddRange(targetQuestionPool.planComunQuestions);
        
        targetQuestionPool.allQuestions = allQuestionsList.ToArray();
    }
    
    private void ClearQuestionPool()
    {
        // Buscar QuestionPool automáticamente
        string[] guids = AssetDatabase.FindAssets("t:QuestionPool");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No se encontró ningún QuestionPool.", "OK");
            return;
        }
        
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        QuestionPool targetQuestionPool = AssetDatabase.LoadAssetAtPath<QuestionPool>(path);
        
        if (targetQuestionPool == null) return;
        
        bool confirm = EditorUtility.DisplayDialog(
            "Confirmar Limpieza",
            "¿Estás seguro de que quieres limpiar todas las preguntas del Question Pool?",
            "Sí",
            "No"
        );
        
        if (!confirm) return;
        
        targetQuestionPool.allQuestions = new Question[0];
        targetQuestionPool.civilQuestions = new Question[0];
        targetQuestionPool.computacionQuestions = new Question[0];
        targetQuestionPool.ambientalQuestions = new Question[0];
        targetQuestionPool.obrasQuestions = new Question[0];
        targetQuestionPool.electricaQuestions = new Question[0];
        targetQuestionPool.planComunQuestions = new Question[0];
        
        EditorUtility.SetDirty(targetQuestionPool);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Question Pool limpiado exitosamente.");
    }
}
