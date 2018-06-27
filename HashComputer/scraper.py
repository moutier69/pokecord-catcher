from PIL import Image
from bs4 import BeautifulSoup
from io import BytesIO
import requests
import json

def download(pId, pName):
    pName = pName.capitalize()
    pId = str(pId).rjust(3, "0")
    
    res = requests.get(f'https://bulbapedia.bulbagarden.net/wiki/File:{pId}{pName}.png')
    if res.status_code == 200:
        soup = BeautifulSoup(res.content)
        img = soup.find('img', {'alt': f'File:{pId}{pName}.png'})
        if img:
            r = requests.get('https:' + img['src'])
            image = Image.open(BytesIO(r.content))
            print('Saved ' + pName)
            image.save(f'Pokemon/{pName}.png', format='PNG')
            return

    print(pName + ' not found')
            
data = requests.get('https://pokeapi.co/api/v2/pokemon/?limit=6969').json()['results']
data = [x for x in data if not x['name'].endswith('-mega')]

for i, c in enumerate(data):
    print('Downloading ' + c['name'])
    download(i + 1, c['name'])


