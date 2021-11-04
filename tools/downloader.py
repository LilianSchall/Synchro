import os
import argparse, sys
try:
    import yt_dlp
except ImportError:
    print("Missing yt_dlp package")


def downloader(file_name, url):
    def renameFile(current_file_name):
        print("Current file name: " + current_file_name)
        os.rename(current_file_name,file_name)
        print("New file name: " + file_name)

    codec = file_name.split(".")[1]
    ydl_opts = \
    {
        'format': 'bestaudio/best',
        'postprocessors':
        [{
            'key': 'FFmpegExtractAudio',
            'preferredcodec': codec,
            'preferredquality': '192',
        }],
        'post_hooks': [renameFile]
    }
    #Checking if file name and url has been well transferred
    print("Filename is:" + file_name)
    print("url is: " + url)

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])
    print("File downloaded.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()

    parser.add_argument('--filename', help="The name of the file that will be downloaded")
    parser.add_argument('--url', help="The url to download the audio from")

    args = parser.parse_args()
    downloader(args.filename, args.url)
