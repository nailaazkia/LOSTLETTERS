using UnityEngine;
using System;

public static class CoinManager
{
    private const string CoinKey = "TotalCoins";

    // Event dipanggil setiap kali jumlah koin berubah
    public static event Action<int> OnCoinsChanged;
    
    // Event dipanggil khusus saat koin dikurangi (untuk animasi floating text)
    public static event Action<int> OnCoinsDeducted;

    public static int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinKey, 0);
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0) return;

        int currentCoins = GetCoins();
        PlayerPrefs.SetInt(CoinKey, currentCoins + amount);
        PlayerPrefs.Save();
        
        // Panggil event untuk update UI
        OnCoinsChanged?.Invoke(GetCoins());
        Debug.Log("Koin bertambah: " + amount + ". Total sekarang: " + GetCoins());
    }

    public static bool UseCoins(int amount)
    {
        if (amount <= 0) return false;

        int currentCoins = GetCoins();
        if (currentCoins >= amount)
        {
            PlayerPrefs.SetInt(CoinKey, currentCoins - amount);
            PlayerPrefs.Save();
            
            // Panggil event untuk update UI
            OnCoinsChanged?.Invoke(GetCoins());
            OnCoinsDeducted?.Invoke(amount);
            Debug.Log("Menggunakan " + amount + " koin. Sisa: " + GetCoins());
            return true;
        }

        Debug.Log("Koin tidak cukup! Butuh: " + amount + ", Tersedia: " + currentCoins);
        return false;
    }

    public static void ResetCoins()
    {
        PlayerPrefs.SetInt(CoinKey, 0);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(0);
        Debug.Log("Semua koin di-reset menjadi 0.");
    }
}
