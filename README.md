# PROJET EASYSAVE

EasySave est une application console en .NET Core permettant d'automatiser des sauvegardes de fichiers en mode complet ou différentiel. Elle offre une exécution via ligne de commande, un suivi en temps réel et la génération de logs au format JSON, le tout compatible avec des environnements francophones et anglophones.

Les mises à jour mineures et patches sont gérées via des pull requests avant d’être intégrées. Les mises à jour majeures suivent un workflow spécifique : elles sont d'abord développées sur la branche develop, puis fusionnées dans main une fois prêtes.

Les fichiers de log sont enregistrés dans le dossier \FISA_A3_GL_MOREEL_Remy\EasySave. Vous y trouverez les journaux des sauvegardes classés en tableau d'element JSON. Le format des noms des fichiers est : jj-MM-aaaa.

Le fichier de configuration est enregistré dans le dossier \FISA_A3_GL_MOREEL_Remy\EasySave. Il contient les paramètres de l’application au format JSON, permettant de conserver en mémoire les travaux de sauvegarde créés.

[Documentation utilisateur](EasySave/UserDocumentation.md)

<br>
<br>

# EASYSAVE PROJECT

EasySave is a .NET Core console application for automating file backups in full or differential mode. It offers command-line execution, real-time monitoring and log generation in JSON format, all compatible with French and English-speaking environments.

Minor updates and patches are managed via pull requests before being integrated. Major updates follow a specific workflow: they are first developed on the develop branch, then merged into main once ready.


Log files are saved in the \FISA_A3_GL_MOREEL_Remy\EasySave folder. There you will find the backup logs classified in JSON element array. The file name format is: dd-MM-yyyy.

The configuration file is saved in the \FISA_A3_GL_MOREEL_Remy\EasySave folder. It contains the application settings in JSON format, allowing created backup jobs to be stored in memory.

[User documentation](EasySave/UserDocumentation.md)