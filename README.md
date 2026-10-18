# 📡 Smart-X IoT Gateway

A hybrid IoT platform for monitoring and managing distributed sensor networks. Built with **ASP.NET Core Web API** (backend) and **ASP.NET Core MVC** (frontend). It registers devices, records live telemetry, flags abnormal readings, supports manual actuator commands and Undo, and displays predictive **Suggested Actions**.

---

## 🎯 Project Overview

The Smart-X IoT platform ingests multi-typed telemetry data (floats for soil moisture, integers for power wattage, booleans for valve states) from a mesh of microcontrollers (ESP32), validates it against configurable thresholds, and visualises it through an interactive web dashboard.

---

## 📁 Part 2 Project Structure

```
SmartX_Platform/
│
├── SmartX_Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   └── Services/
│       ├── TelemetryDispatchService.cs   # FIFO & Priority Queues
│       ├── CommandOperationsService.cs   # Stack & Undo
│       ├── LiveStreamStore.cs            # Dictionaries & HashSets
│       └── PredictiveInsightsService.cs  # Predictive Engine
│
└── SmartXMvc/
    ├── Controllers/
    ├── Models/
    ├── Services/
    └── Views/
```

---

## 🚀 Getting Started

### ✅ Prerequisites

Ensure the following are installed before running:

| Requirement | Minimum Version | Download |
|---|---|---|
| **.NET SDK** | 10 or later | https://dotnet.microsoft.com/download |
| **SQL Server LocalDB** | 2019 or later | Bundled with Visual Studio |
| **EF Core CLI tools** | Latest | `dotnet tool install --global dotnet-ef` |
| **Visual Studio** (optional) | 2022 | https://visualstudio.microsoft.com/ |
| **Git** | Any | https://git-scm.com/ |

To verify:

```bash
dotnet --version
dotnet ef --version
```

### 1. Clone the Repository

```bash
git clone https://github.com/EMECPE/prog7312-2026-poe-ghanief.git
cd prog7312-2026-poe-ghanief
```

### 2. Restore Dependencies

```bash
# Run from the repository root
dotnet restore SmartX_Api
dotnet restore SmartXMvc
```

### 3. Configure the Connection String

In `SmartX_Api/appsettings.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SmartXDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 4. Configure the API Base URL

In `SmartXMvc/appsettings.json`:

```json
"SmartXApi": {
    "BaseUrl": "http://localhost:5095"
}
```

### 5. Apply Database Migrations

From the repository root:

```bash
dotnet ef database update --project SmartX_Api
```

### 6. Run the Application

In **Terminal 1**, from the repository root:

```bash
dotnet run --project SmartX_Api --launch-profile http
```

Wait until you see:

```
Now listening on: http://localhost:5095
Application started.
```

**Leave this terminal running.**

In **Terminal 2**, from the repository root:

```bash
dotnet run --project SmartXMvc --launch-profile http
```

Wait until you see:

```
Now listening on: http://localhost:5109
Application started.
```

### 7. Open the Application

Navigate to **http://localhost:5109** in your browser.

---

## 🖥️ Using the Application

| Page | Main Function |
|---|---|
| **Dashboard** | View live telemetry, sensor counts and charts, updating every **5 seconds**. |
| **Sensors** | Register, edit, delete, disconnect or reconnect devices. |
| **Telemetry** | Submit sensor readings manually. |
| **Command Stream** | View alerts, device trends, **Manual Override**, **Undo**, command history and **Suggested Actions**. |
| **Files** | Upload, download or delete sensor documents. |

---

## 🧱 Data Structures

| Structure | Implementation File | Use |
|---|---|---|
| **`Queue<T>`** | `TelemetryDispatchService.cs` | FIFO processing of normal telemetry packets. |
| **`PriorityQueue<TElement,TPriority>`** | `TelemetryDispatchService.cs` | Processes critical anomalies before standard packets. |
| **`Stack<T>`** | `CommandOperationsService.cs` | Stores command history for Last-In-First-Out Undo. |
| **`Dictionary<TKey,TValue>`** | `LiveStreamStore.cs` | Fast lookup of registered devices by unique identifier. |
| **`SortedDictionary<TKey,TValue>`** | `LiveStreamStore.cs` | Maintains telemetry timeline points in timestamp order. |
| **`HashSet<T>`** | `LiveStreamStore.cs` | Tracks unique alert and disconnected-device IDs without duplicate counts. |

---

## 🧠 Predictive Action Engine

The **Suggested Actions** panel on **Command Stream** is provided by `PredictiveInsightsService.cs` and refreshes approximately every **5 seconds**.

| Insight | Trigger | What Appears |
|---|---|---|
| **Anomaly** | Abnormal reading within approximately **15 seconds** | **Investigate sensor** |
| **Disconnected** | No reading for more than **25 seconds**, or no readings ever | **Check connection** |
| **SearchPattern** | Same exact device ID searched **3 times within 5 minutes** | **Frequently viewed sensor** |
| **CommandPattern** | Two separate resolved anomalies for the same sensor and type, each followed by the same successful, non-undone actuator command within **10 minutes**, followed by a new matching anomaly | **Past pattern** recommendation with a **Prepare** button |

---

## 🧪 Testing the Application

Use the **Command Stream** page and existing registered device identifiers for the following tests.

### 📥 Test 1: FIFO Queue

**Objective:** Verify that normal telemetry readings are processed in First-In-First-Out (FIFO) order.

**Steps:**
1. Navigate to the **Telemetry** page.
2. Select `SENSOR-TEMP-001`.
3. Submit three temperature readings in sequence: **24°C, 25°C and 26°C**.
4. Open **Command Stream → Live Device Monitor**.
5. Find the sensor and click **View Trend** to inspect its readings.

**Expected Result:** All three readings are successfully recorded and displayed in chronological order. The `_standard` queue processes normal telemetry using FIFO, meaning the first reading added is the first to be processed.

---

### 🚨 Test 2: Priority Queue

**Objective:** Verify that abnormal sensor readings are identified as critical and given higher processing priority.

**Steps:**
1. Navigate to the **Telemetry** page.
2. Select `SENSOR-POWER-005`.
3. Submit a normal power reading of **1800 W**.
4. Submit an abnormal power reading of **6000 W**.
5. Immediately open **Command Stream** and check for the sensor's alert.
6. Navigate to **Suggested Actions** to view the anomaly recommendation.

**Expected Result:** The system identifies the **6000 W** reading as an anomaly and displays a critical alert. The `_critical` priority queue prioritises abnormal readings over normal telemetry.

---

### ↩️ Test 3: Stack and Undo

**Objective:** Verify that the Stack follows the Last-In-First-Out (LIFO) principle, meaning the most recent command is undone first.

**Steps:**
1. Navigate to **Command Stream → Manual Override**.
2. Select `SENSOR-VALVE-011` and issue the **Open** command.
3. Issue a second command to **Close** the same valve.
4. Click **Undo last** to reverse the Close command, reopening the valve.
5. Click **Undo last** again to reverse the Open command, restoring the valve to its original state.

**Expected Result:** The system reverses commands in the opposite order they were executed, demonstrating the LIFO behaviour of the Stack.

---

### 🔎 Test 4: Dictionary

**Objective:** Verify that registered sensors can be quickly retrieved using their unique device identifiers.

**Steps:**
1. Navigate to **Command Stream → Live Device Monitor**.
2. Enter `SENSOR-TEMP-001` in the search bar.
3. Check that the correct sensor appears in the results.

**Expected Result:** The system successfully finds and displays the registered sensor using the `_devices` dictionary, which stores devices by their unique identifiers.

---

### 📈 Test 5: Sorted Dictionary

**Objective:** Verify that sensor telemetry readings are stored and displayed in chronological order.

**Steps:**
1. Navigate to the **Telemetry** page.
2. Select `SENSOR-TEMP-001` and submit three readings: **24°C, 26°C and 28°C**.
3. Open **Command Stream → Live Device Monitor**.
4. Find `SENSOR-TEMP-001` and click **View Trend**.
5. Check the graph to see how the sensor values changed over time.

**Expected Result:** The graph displays the sensor's readings in chronological order, from oldest to newest. The `SortedDictionary` organises telemetry data by timestamp.

---

### 🧩 Test 6: HashSet

**Objective:** Verify that disconnected sensors are tracked without duplicate entries.

**Steps:**
1. Navigate to the **Sensors** page.
2. Find `SENSOR-TEMP-002` and click **Disconnect**.
3. Wait approximately **25 to 30 seconds**.
4. Open **Command Stream → Live Device Monitor**.
5. Select the **Disconnected** filter and locate the sensor.
6. Reconnect the sensor after testing.

**Expected Result:** The disconnected sensor appears only once in the disconnected device count. The `HashSet` stores unique sensor identifiers, preventing duplicates.

---

### 🔍 Test 7: Search Pattern

**Objective:** Verify that the predictive engine identifies sensors that users search for repeatedly.

**Steps:**
1. Navigate to **Command Stream → Live Device Monitor**.
2. Search for the full device identifier `SENSOR-TEMP-001`.
3. Clear the search box and repeat the same search two more times within **5 minutes**.
4. Scroll down to the **Suggested Actions** section.

**Expected Result:** The system displays a **Frequently viewed sensor** recommendation for `SENSOR-TEMP-001`, indicating that the sensor has been searched for multiple times.

---

### 🌡️ Test 8: Anomaly Insight

**Objective:** Verify that the system detects abnormal telemetry readings and generates recommendations.

**Steps:**
1. Navigate to the **Telemetry** page.
2. Select `SENSOR-TEMP-001`.
3. Enter a temperature reading of **40°C** and submit it.
4. Immediately open **Command Stream → Suggested Actions**.
5. Check for a high-priority recommendation.

**Expected Result:** The system identifies the temperature as outside its normal range and displays an **Investigate sensor** recommendation. The alert is normally visible for approximately **15 seconds** after the abnormal reading.

---

### 🤖 Test 9: Learned Command Recommendation

**Objective:** Verify that the predictive engine learns from previous actuator commands and recommends actions when the same sensor problem occurs again.

**Preparation:** On the **Sensors** page, disconnect `SENSOR-TEMP-001` to pause automatic telemetry. Keep `SENSOR-VALVE-011` available.

**Steps:**

**First Occurrence:**
1. Submit a temperature reading of **40°C** for `SENSOR-TEMP-001`.
2. Navigate to **Command Stream → Manual Override** and issue **TurnOn** to `SENSOR-VALVE-011`.
3. Submit **24°C** for the temperature sensor to resolve the anomaly.

**Second Occurrence:**
1. Submit **40°C** again for the same sensor.
2. Issue the same **TurnOn** command to the same valve.
3. Submit **24°C** to resolve the anomaly.
4. Do not undo either command.

**Third Occurrence:**
1. Submit **40°C** again.
2. Immediately navigate to **Command Stream → Suggested Actions**.
3. Check whether the system recommends the previously used command.
4. Click **Prepare** to populate the recommended command without executing it.
5. Reconnect `SENSOR-TEMP-001` after testing.

**Expected Result:** The system recognises that **TurnOn** was previously used during two separate temperature anomalies and displays a **Past pattern: TurnOn on SENSOR-VALVE-011** recommendation. This demonstrates that the predictive engine uses historical commands to suggest actions for recurring sensor problems.

---

## 📺 Video Demo

Watch the project walkthrough and feature demonstration on YouTube:



---

## 👤 Author

* **Student:** Ghanief Batchelor
* **Student Number:** ST10440143
* **Module:** PROG7312 - Part 1
* **Submission Date:** 14 September 2026
