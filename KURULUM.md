# 🎮 Stardew Valley Clone - Kurulum Rehberi

## ✅ Sistem Gereksinimleri

- **İşletim Sistemi:** Windows 10/11
- **GPU:** NVIDIA GTX 1050 Ti veya üstü (OpenGL 3.3+ desteği)
- **.NET SDK:** 8.0 veya üstü
- **RAM:** 4 GB (8 GB önerilen)

## 📥 1. .NET SDK Kurulumu

### Yüklenmiş mi Kontrol Et:
```cmd
dotnet --version
```

Eğer `8.0.404` gibi bir şey gösteriyorsa, **2. adıma geç**. ✅

### .NET SDK İndir ve Kur:

**Direkt Link:**
```
https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.404-windows-x64-installer
```

1. Linki tarayıcıda aç
2. İndirilen `dotnet-sdk-8.0.404-win-x64.exe` dosyasını çalıştır
3. "Install" butonuna tıkla
4. Kurulum bitince **Command Prompt'u kapat ve yeniden aç**
5. Kontrol et: `dotnet --version`

---

## 🚀 2. Oyunu Çalıştırma

### Adım 1: Proje Klasörüne Git

```cmd
cd C:\Users\KULLANICI_ADI\Desktop\StardewValleyClone
```

(KULLANICI_ADI yerine kendi kullanıcı adını yaz)

### Adım 2: Bağımlılıkları İndir

```cmd
dotnet restore
```

**İlk kez 1-2 dakika sürebilir.** Şunları indirecek:
- Silk.NET (OpenGL wrapper)
- ImageSharp (Görsel işleme)
- Newtonsoft.Json (Save/Load)

### Adım 3: Oyunu Başlat

```cmd
dotnet run
```

**İlk çalıştırma ~30 saniye sürebilir (derleme yapacak).**
**Sonraki çalışmalar 5 saniye içinde başlar.**

---

## 🎮 Oyun Kontrolleri

### Hareket:
- **WASD** veya **Arrow Keys** (↑↓←→)

### Eylemler:
- **C** veya **Sol Mouse Tuşu**: Tool kullan / Hasat yap
- **1-9**: Inventory slot seç

### Başlangıç Envanteri:
1. **Hoe (Çapa)** - Toprak işle
2. **Watering Can (Sulama)** - Toprak sula
3. **Parsnip Seeds** - Tohum (10 adet)

---

## 🌱 Nasıl Oynanır?

### Farming Döngüsü:

1. **Toprak İşle:**
   - Slot 1'i seç (Hoe)
   - Bir karenin önünde dur
   - **C** tuşuna bas

2. **Tohum Ek:**
   - Slot 3'ü seç (Parsnip Seeds)
   - İşlenmiş toprağın önünde dur
   - **C** tuşuna bas

3. **Sula:**
   - Slot 2'yi seç (Watering Can)
   - Ekili toprağın önünde dur
   - **C** tuşuna bas

4. **Bekle:**
   - Zaman otomatik ilerliyor
   - Parsnip 4 gün içinde büyür
   - Her gün sulamayı unutma!

5. **Hasat:**
   - Bitki sarı olunca hazır
   - **C** tuşuna bas
   - Ürün envanterine eklenir!

---

## ⏰ Zaman Sistemi

- **10 dakika oyun içi = 7 saniye gerçek zaman**
- Gün **6:00 AM** başlar, **2:00 AM** biter
- Her mevsim **28 gün**
- Mevsimler: Spring → Summer → Fall → Winter

---

## 📊 Performans

### GTX 1050 Ti Beklenen Performans:
- **FPS:** 60 (VSync açık)
- **RAM:** 500-700 MB
- **GPU Kullanımı:** %10-15
- **Sıcaklık:** 50-55°C

Oyun çok hafif, rahatça çalışır! 🚀

---

## 🐛 Sorun Giderme

### "dotnet: command not found"
- .NET SDK kurulmamış
- Command Prompt'u yeniden başlat
- PATH'e ekle: `setx PATH "%PATH%;C:\Program Files\dotnet"`

### "Could not load file or assembly 'Silk.NET'"
```cmd
dotnet clean
dotnet restore
dotnet run
```

### Pencere açılıyor ama siyah ekran
- NVIDIA sürücülerini güncelle
- NVIDIA Kontrol Paneli → StardewValleyClone.exe → "High-performance"
- Windows Güç Planı → "High Performance"

### Build hatası: "Syntax error"
- Tüm dosyaların güncel olduğundan emin ol
- ZIP'i yeniden çıkart
- `dotnet clean` sonra `dotnet build`

---

## 📁 Proje Yapısı

```
StardewValleyClone/
├── Core/           - Ana oyun döngüsü
├── Engine/         - Time, Input, Resource, Scene yönetimi
├── Graphics/       - Rendering, Texture, Shader
├── GameObjects/    - Player, NPC, Farm scene
├── Systems/        - Farming, Fishing, Mining, Crafting, Inventory, Save/Load
├── UI/             - HUD ve menüler
├── Assets/         - Textures, Sounds, Data (şu anda placeholder'lar)
└── Program.cs      - Entry point
```

**Toplam:** 21 C# dosyası, ~3500 satır kod

---

## 🎯 Özellikler

### ✅ Tamamlandı:
- Player hareketi ve animasyon
- Tile-based farming (till, plant, water, harvest)
- 8 farklı ürün tipi
- Zaman ve mevsim sistemi
- Inventory (36 slot)
- Enerji ve para sistemi
- NPC ve dialog sistemi
- İlişki/arkadaşlık sistemi
- Crafting sistemi
- Balık tutma
- Madencilik (120 kat)
- Save/Load (JSON)
- UI/HUD

### 🚧 Eklenebilir:
- Gerçek sprite'lar (şu anda placeholder)
- Ses ve müzik
- Daha fazla NPC
- Building sistemi
- Hayvan yetiştirme
- Şehir ve diğer lokasyonlar
- Quest sistemi
- Multiplayer

---

## 💡 İpuçları

1. **İlk günler:**
   - Tüm parsnip tohumlarını ek
   - Her gün sula
   - 4 gün sonra hasat yap
   - Satış yapıp daha fazla tohum al

2. **Enerji yönetimi:**
   - Her eylem (çapa, sulama) enerji harcar
   - Enerji bitince dikkatli ol
   - Sabah enerjin yenilenir

3. **Para kazan:**
   - Ürünleri sat
   - Balık tut
   - Madende maden topla

---

## 📞 Destek

Sorun yaşarsan:
1. README.md dosyasını oku
2. KURULUM.md dosyasını kontrol et
3. `dotnet clean && dotnet restore` dene

---

## 🎮 İyi Oyunlar!

**Monster Abra GTX 1050 Ti ile mükemmel çalışacak!** 💪

Keyifli farmlaşmalar! 🌾🚜
