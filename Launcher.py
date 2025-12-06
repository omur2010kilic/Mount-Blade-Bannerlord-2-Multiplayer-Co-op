import tkinter as tk
from tkinter import messagebox
import subprocess
import os


STEAM_APP_ID = "261550"  # Bannerlord Steam AppID

def play_game():
    try:
        
        subprocess.Popen(["steam://rungameid/" + STEAM_APP_ID])
        messagebox.showinfo("Bilgi", "Game has Started")
    except Exception as e:
        messagebox.showerror("Error", f"Something went wrong\n{e}")

def exit_launcher():
    root.destroy()

# Tkinter form
root = tk.Tk()
root.title("BANNERLORD CO-OP")
root.geometry("500x500")
root.resizable(False, False)

# Başlık
label_title = tk.Label(root, text="BANNERLORD CO-OP (TURKCE!)", font=("Arial", 16, "bold"))
label_title.pack(pady=20)

# Play butonu
btn_play = tk.Button(root, text="PLAY", width=20, command=play_game)
btn_play.pack(pady=5)

# Exit butonu
btn_exit = tk.Button(root, text="EXIT", width=20, command=exit_launcher)
btn_exit.pack(pady=5)
root.mainloop()

