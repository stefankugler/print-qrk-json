# QRK Registrierkasse als Serverdienst für alte Kassenlösungen
Die Software *[QRK Registrierkasse](http://www.ckvsoft.at/)* ist eine einfache Kasse, die nach Angaben der Projekthomepage der österreichischen Registrierkassensicherheitsverordnung entspricht.
Neben der typischen Kassenfunktion nimmt die Kasse im Server-Modus Rechnungsdaten per JSON-Datei entgegen. Mit dieser Funktion können Rechnungen aus anderen Programmen heraus erstellt werden. Hier wird beschrieben, wie Bons aus POS-Software der Codebasis POSper, Chromis POS, uniCenta POS,... mit Hilfe von QRK erstellt und signiert werden können.
Das Format der JSON-Datei wurde im [Forum](http://www.ckvsoft.at/forum/qrk-fragen-und-antworten/anbindung-an-boniersystem/#post-648) veröffentlicht. Die Anpassung der Bons der genannten Kassensysteme erfolgt über die Template-Engine *Apache Velocity*. Die Vorlage `Printer.Ticket[.pos-software]` erstellt ein JSON-Objekt für QRK.

Die Beschreibung wurde unter Windows XP, 7 und 10 mit der POS-Software Chromis POS und POSper getestet. Für Linux gibt es einen Lösungsansatz für Chromis POS und POSper. Rückmeldung zu anderen Kassenlösungen arbeite ich gerne ein!
Auf der [QRK-Homepage](http://www.ckvsoft.at/kb/installation-qrk-chromispos/) sind auch Infos zur Verknüpfung von Chromis POS und QRK zu finden. Fragen/Ergänzungen bitte im [QRK-Forum](http://www.ckvsoft.at/forum/) posten!

Durch die Verwendung eines neuen Druckers in der POS-Software für den Export nach QRK können alle Ausdrucke des Kassensystems (ausgenommen Kassabons) wie bisher getätigt werden (Abschluss etc). Ein Nachdruck des Bons müsste aber über QRK erfolgen, je nach Konfiguration des Kassensystems würde der Bon ansonsten ggf. in QRK neu verbucht werden.

**Haftungsausschluss:** Der Einsatz der genannten Softwareprodukte erfolgt auf eigenes Risiko!

# Chromis POS
## Linux
- Neuen unbenutzten Drucker (z.B. Drucker 2) konfigurieren: Typ: `epson`, Modus: `file`, Port: `/opt/chromis.pipe`
- Template Printer.Ticket mit (Printer.Ticket.chromis) ersetzen und für den richtigen Drucker bearbeiten z.B. `<ticket printer="2">`. Altes Template ggf. sichern.
- Pipe anlegen: `mkfifo /opt/chromis.pipe`
- Skript [bon2json.sh](bon2json.sh) anlegen und ausführbar machen: `chmod a+x bon2json.sh`
- Das Verzeichnis für den Server-Modus anlegen und im Skript ggf. ändern (im Beispiel `/opt/json`). Das Verzeichnis muss für den Kassa-User beschreibbar sein.
- Skript ausführen (z.B. in die `/etc/rc.local` als User mit `su <kassauser> -c '/opt/bon2json.sh'` oder eine andere Autostart-Funktion des Desktop-Environments nutzen)
- Berechtigungen anpassen (Pipe für Kassa-User lesbar machen `chmod 666 chromis.pipe`)
- ESC-Code für die Kassenlade kann mit dem Template für den Export über den alten Drucker mitgeschickt werden (siehe Template (Printer.Ticket.posper-kitchen), dort werden zwei Druckaufträge für zwei unterschiedliche Drucker erstellt, der Befehl für das Öffnen der Kassenlade lautet `<opendrawer/>`), oder im Skript (bon2json.sh) eingefügt werden, z.B. `echo -e -n "\x1b\x70\x30\x40\x50" > /dev/usb/lp0` für einen USB-Drucker (ungetestet)
- QRK konfigurieren
- QRK im Server-Modus starten mit `qrk --servermode`
- Fehlersuche: siehe [Fehlersuche](#fehlersuche)

## Windows
- Die Software *[Multi File Port Monitor](https://sourceforge.net/projects/mfilemon/)* installieren
- Lege einen neuen Drucker an, wähle *Einen lokalen Drucker hinzufügen* und erstelle einen neuen Anschluss vom Typ *Multi File Port Monitor*.
  - Name des Ports: `kasse`
  - Output Path: `C:\CkvSoft\import`
  - Filename Pattern: `bon-%0000i.json`. Alternativ `%Y%m%d%H%n%s.json` für eine Benennung mit Zeitstempel. Weitere Variablen sind in der Port-Konfiguration von des Druckers über die Schaltfläche `?` zu finden.
  - Overwrite existing files: `aktiviert`
  - Druckertreiber: HP LaserJet 4100 PCL6 (egal?)
  - Name des Druckers: `kasse`
  - Unter Windows 10 lässt sich der Drucker im letzten Schritt nicht anlegen, der Anschluss sollte aber dennoch funktionieren
- Neuen unbenutzten Drucker (z.B. Drucker 2) konfigurieren: Typ: `epson`, Modus: `raw`, Printer: `kasse` (so wie der Name des Ports, der Name des Druckers sollte egal sein und er kann ggf. nach Erstellung des Ports wieder gelöscht werden)
- Einstellungen speichern und Chromis POS beenden. Öffne die Konfigurationsdatei von Chromis POS (zu finden unter `C:\User\<username>\chromispos.properties`) mit einem Editor und finde die Zeile `machine.printer[.x]=epson\:raw,kassa` (`x`steht für die Nummer des gewählten Druckers, beginnend bei 2, der erste Drucker heißt nur `machine.printer`). Ändere den Wert von `epson\:raw,kassa` auf `file\:raw,kassa`. Die Einstellung `file` erlaubt in Chromis POS keine weiteren Einstellungen, die Einstellungen aus der Konfigurationsdatei werden aber übernommen. Achtung bei der Änderung weiterer Einstellungen, ggf. muss dieser Wert neu angepasst werden (Bug/Featre?).
- Template Printer.Ticket mit (Printer.Ticket.chromis) ersetzen und für den richtigen Drucker bearbeiten z.B. `<ticket printer="2">`. Altes Template ggf. sichern.
- Das öffnen der Kassenlade kann über den Druckertreiber des Bondruckers oder über ein zusätzliches Template für den alten Anschluss des Bondruckers erfolgen (siehe Template (Printer.Ticket.posper-kitchen), dort werden zwei Druckaufträge für zwei unterschiedliche Drucker erstellt, der Befehl für das Öffnen der Kassenlade lautet `<opendrawer/>`)
- QRK konfigurieren
- QRK im Server-Modus starten mit `qrk --servermode`
- Fehlersuche: siehe [Fehlersuche](#fehlersuche)

## Fehlersuche
- Prüfe, ob die POS-Software eine JSON-Datei "druckt": Der Inhalt dieser Ausgabedatei sollte von der Struktur her der Beispieldatei (testbon.json) ähneln (nur kompakter formatiert). Ist das nicht der Fall, liegt der Fehler bei der Konfiguration des POS-Systems
- QRK beanstandet das Format der JSON-Datei: Öffne die Datei mit einem Editor (z.B. Notepad++, der Windows-Editor hat Probleme mit der Anzeige der Steuerzeichen und stellt den kompletten Inhalt falsch dar). Befinden sich in der Datei "komische" Zeichen, so wurde der falsche Druckertyp im POS-System ausgewählt.
