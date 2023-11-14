from kivy.app import App,async_runTouchApp
from kivy.uix.widget import Widget
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.scrollview import ScrollView
from kivy.graphics import *
from kivy.core.window import Window
from kivy.clock import Clock
from kivy.uix.popup import Popup
from kivy.uix.label import Label
from kivy.uix.button import Button
from kivy.uix.screenmanager import ScreenManager, Screen,FadeTransition
from random import choice,shuffle,randint
from functools import partial
from datetime import time,datetime


class Monstor:
    hp = 20
    heat_range = (0,20)

class Hero:
    class HeroIsDead(Exception):
        pass

    INFO = {
        'coins':0,
        'monsters':0,
        'time':datetime.now(),
        'hp':0
    }
    def __init__(self,hp,heat_range):
        self.__hp=hp
        self.heat=heat_range
        self.INFO['hp'] = hp

    def kick(self):
        return randint(*self.heat)

    @property
    def hp(self):
        return self.__hp

    @hp.setter
    def hp(self,value):
        self.__hp+=value
        self.INFO['hp']=self.__hp
        if self.__hp<=0:
            raise self.HeroIsDead()

    def get_info(self,title):
        return title+'\n    '+'\n    '.join([f'{tup[0]} : {tup[1]}' for tup in self.INFO.items()])

    def _get_screen(self,title,ins):
        self.INFO['time'] = datetime.now() - self.INFO['time']
        app.sm.current = 'gameover'
        app.label.text += self.get_info(title)

class Field(Widget):
    TRAP_SIGN = ' '  # обозначение пути
    WALL_SIGN = '#'  # обозначение стены

    def _close_popup(self,title,ins):
        self.popup.dismiss()
        self.my_hero._get_screen(title,ins)

    def __init_popup(self):
        bx = BoxLayout(orientation='vertical')
        bx.add_widget(Label(text='[color=#25D469]Вы нашли выход!!![/color]', font_size=35, markup=True))  # 25D469
        in_bx = BoxLayout(size_hint=[1, None], height=70, spacing=5)
        btn = Button(text='Остаться', background_normal='', background_color=[26 / 255, 26 / 255, 26 / 255, 1])
        self.popup = Popup(title='Exit',
                           content=bx,
                           size_hint=[None, None], size=[350, 350],
                           auto_dismiss=False,
                           separator_color=[26 / 255, 26 / 255, 26 / 255, 1],
                           title_align='center', title_size=27,
                           background='exit.webp',
                           separator_height=5,
                           title_color=[37 / 255, 212 / 255, 105 / 255, 1])

        btn.bind(on_press=self.popup.dismiss)
        in_bx.add_widget(btn)
        in_bx.add_widget(Button(text='Уйти', background_normal='', background_color=[26 / 255, 26 / 255, 26 / 255, 1],
                                on_press=partial(self._close_popup,'[size=20][i][b]YOUR WIN!!![/b][/i][/size]')))
        bx.add_widget(in_bx)

    def __monstor_fight_popup(self):
        bx = BoxLayout(orientation='vertical')
        bx_h = BoxLayout(spacing=100)

        self.wd_hero = Label(text=f'\nHERO - {self.my_hero.hp}')
        self.wd_monstor = Label(text=f'\nMONSTOR - {self.monstor_hp}')

        bx_h.add_widget(self.wd_hero)
        bx_h.add_widget(self.wd_monstor)
        bx.add_widget(bx_h)
        bx.add_widget(btn:=Button(size_hint=[None,None],height=70,width=350,pos_hint={'center_x':.5},
                             background_normal='',background_color=[1,1,1,.5],text='Атаковать',font_size=25))
        btn.bind(on_press=self.heat)
        self.mnstr_popup = Popup(title='На вас напал монстор!!!',
                                 content=bx,size_hint=[None,None],size=[816,464],
                                 background='fight.png',separator_height=0,
                                 title_align='center', title_size=27,)

    def heat(self,ins):

        self.monstor_hp -= self.my_hero.kick()

        if self.monstor_hp<=0:
            self.wd_monstor.text = f'\nMONSTOR - {0}'
            self.mnstr_popup.dismiss()
            self.event_monstor = Clock.schedule_interval(self._update_pos_monstors, 1)
            self.monstor_hp=Monstor.hp
            self.canvas.after.remove(self.monstor)
            self.monstors_dct.pop(self.monstor)
            self.my_hero.INFO['monsters']+=1
            Clock.schedule_once(lambda time:setattr(self.wd_monstor,'text',f'\nMONSTOR - {self.monstor_hp}') ,1/5)


        else:
            try:
                self.my_hero.hp=-randint(*Monstor.heat_range)
                self.wd_hero.text=f'\nHERO - {self.my_hero.hp}'
                self.wd_monstor.text = f'\nMONSTOR - {self.monstor_hp}'
            except Hero.HeroIsDead:
                self.mnstr_popup.dismiss()
                self.my_hero.INFO['hp']=0
                self.my_hero._get_screen('[size=20][i][b]GAME OVER[/b][/i][/size]',None)

    def __init__(self,**kwargs):
        super().__init__(**kwargs)
        self._keyboard = Window.request_keyboard(
            self._keyboard_closed, self, 'null')
        self._keyboard.bind(on_key_down=self._on_keyboard_down)
        self.X, self.Y = 25, 25
        self.__current_row = 0
        self.__current_col = 0
        self.my_hero = Hero(100,(10,20))
        self.monstor_hp = Monstor.hp
        self.__monstor_fight_popup()
        self.__init_popup()
        self.color_matrix = []
        self.free_blocks = set()
        self.coins_dct=dict()
        self.monstors_dct=dict()
        matrix = {(0, 0): [(0, 0)]}  #
        self.field = [[self.WALL_SIGN] * self.X for _ in range(self.Y)]  # поле лабиринта
        self.createLab(matrix)
        self.size_hint = [None,None]
        self.size = (1000,1000)
        self.render()
        self.free_blocks.discard(self.setup_end())
        self.setup_coins()
        self.setup_monstors()
        del self.free_blocks
        with self.canvas.after:
            block = self.color_matrix[0][0][1]
            self.hero = Rectangle(pos=block.pos,size=block.size,source = 'hero.png')

    @property
    def current_row(self):
        return self.__current_row

    @current_row.setter
    def current_row(self,value):
        self.__current_row=value
        self._setHero()

    @property
    def current_col(self):
        return self.__current_col


    @current_col.setter
    def current_col(self, value):
        self.__current_col = value
        self._setHero()


    @property
    def getCubePos(self):
        cube = self.color_matrix[self.current_row][self.current_col][1]
        return cube.pos

    def checkScroll(self,x,y):
        scrollX,scrollY = self.parent.convert_distance_to_scroll(x, y)
        return scrollX,scrollY

    def _keyboard_closed(self):
        self._keyboard.unbind(on_key_down=self._on_keyboard_down)

    def _on_keyboard_down(self, keyboard, keycode, text, modifiers):#keycoad[1]
        """
        :param keyboard:
        :param keycode:
        :param text:
        :param modifiers:
        :return:

        Обрабатывает нажатия клавиш (влево,вправо,вверх,вниз).
        Если при нажатии на соответсвующую клавишу наш персонаж находится в центре экрана по 'x' или 'y',то мы
        скроллим ScrollView (прокручиваем виджет Field) в соответсвующую сторону на block_size пикселей, т.е. на размер одного блока по 'x'/'y'.
        Если мы находимся на границе виджета и по совместительству на границе скролла,то ничего не делаем.
        Если прокрутка минимальеа - устанавливаем скролл на соответсвующую границу
        """
        match keycode[1]:
            case 'left':
                if self.checkPoint(self.current_row,self.current_col-1,self.WALL_SIGN):
                    self.current_col -= 1
                    if self.to_window(*self.getCubePos)[0]<= Window.size[0]//2 and (tupScroll:=self.checkScroll(self.block_size[0],self.parent.scroll_y)):#В центре экрана по 'x' и расчитываем дистанцию прокрутки влево
                        #мы не на границе скролла, поэтому смело прокручиваем в левую сторону
                        # мы практически на границе (прокрутка меньше, чем длина блока по 'x'), поэтому просто устанавливаем скролл сразу на левую границу
                        distance = -tupScroll[0] / 60 if self.parent.scroll_x-tupScroll[0]>=0 else -(self.parent.scroll_x) / 60
                        self.smoothly_scroll('scroll_x', distance)
            case 'right':
                if self.checkPoint(self.current_row,self.current_col+1,self.WALL_SIGN):
                    self.current_col += 1
                    if self.to_window(*self.getCubePos)[0]>= Window.size[0]//2 and (tupScroll:=self.checkScroll(self.block_size[0],self.parent.scroll_y)):

                        distance = tupScroll[0] / 60 if self.parent.scroll_x+tupScroll[0]<1 else (self.parent.scroll_x) / 60
                        self.smoothly_scroll('scroll_x', distance, limit=1)
            case 'up':
                if self.checkPoint(self.current_row-1,self.current_col,self.WALL_SIGN):
                    self.current_row -= 1
                    if self.to_window(*self.getCubePos)[1]>= Window.size[1]//2 and (tupScroll:=self.checkScroll(self.parent.scroll_x,self.block_size[1])):

                        distance = tupScroll[1] / 60 if self.parent.scroll_y+tupScroll[1]<1 else (self.parent.scroll_y) / 60
                        self.smoothly_scroll('scroll_y', distance,limit=1)
            case 'down':
                if self.checkPoint(self.current_row+1,self.current_col,self.WALL_SIGN):
                    self.current_row += 1
                    if self.to_window(*self.getCubePos)[1] <= Window.size[1] // 2 and (tupScroll := self.checkScroll(self.parent.scroll_x, self.block_size[1])):

                        distance = -tupScroll[1]/60 if self.parent.scroll_y - tupScroll[1] >= 0 else -(self.parent.scroll_y) / 60
                        self.smoothly_scroll('scroll_y',distance)

        return True

    def smoothly_scroll(self,scroll_name,distance,limit=0):
        event = Clock.schedule_interval(partial(self.move, scroll_name, distance , limit=limit), 1 / 60)
        Clock.schedule_once(lambda dt: event.cancel(), 1 / 60 * 60)

    def move(self,scroll,value,timeout,limit=1):
        scroll_xy = getattr(self.parent,scroll)
        if scroll_xy<=limit and limit==0:
            return setattr(self.parent,scroll,limit)
        elif scroll_xy>=limit and limit==1:
            return setattr(self.parent,scroll,limit)
        setattr(self.parent,scroll,scroll_xy+value)





    def _setHero(self):
        self.hero.pos = self.color_matrix[self.current_row][self.current_col][1].pos

        if self.coins_dct.get(pos:=(self.color_matrix[self.current_row][self.current_col][1].pos),False):
            self.canvas.after.remove(self.coins_dct.pop(pos))
            self.my_hero.INFO['coins']+=1


        if self.exit_coords == (self.current_row,self.current_col):
            self.popup.open()

        if (self.current_row,self.current_col) in self.monstors_dct.values():
            self.event_monstor.cancel()
            self.mnstr_popup.open()
            for key,value in self.monstors_dct.items():
                if value==(self.current_row,self.current_col):
                    self.monstor = key
                    break



    def checkPoint(self, row, col, sign=TRAP_SIGN):
        if 0 <= row < self.Y and 0 <= col < self.X and self.field[row][col] != sign:
            return True
        return False

    def createLab(self,matrix):
        while matrix:
            key = choice(list(matrix.keys()))
            for point in matrix[key]:
                self.field[point[0]][point[1]] = self.TRAP_SIGN
            row, col = matrix.pop(key)[0]

            if self.checkPoint(row, col + 2):
                matrix[(row, col + 2)] = [(row, col + 2), (row, col + 1)]
            if self.checkPoint(row, col - 2):
                matrix[(row, col - 2)] = [(row, col - 2), (row, col - 1)]
            if self.checkPoint(row + 2, col):
                matrix[(row + 2, col)] = [(row + 2, col), (row + 1, col)]
            if self.checkPoint(row - 2, col):
                matrix[(row - 2, col)] = [(row - 2, col), (row - 1, col)]

    def render(self,size=(1000,1000)):
        x = size[0] // self.X
        y = size[1] // self.Y
        self.block_size = (x,y)

        start_y = self.size[1]-y
        for row in self.field:
            start_x = 0
            color_row_list=[]
            for col in row:
                if col == self.WALL_SIGN:
                    self.canvas.add(Color(1,1,1,1))
                    color_row_list.append(0)
                    rectangle=Rectangle(pos=(start_x, start_y),size=(x,y),source = 'img.jfif')
                else:
                    self.canvas.add(color:=Color(203/255, 201/255, 195/255,1))

                    rectangle=Rectangle(pos=(start_x, start_y),size=(x,y))
                    color_row_list.append((color,rectangle))
                self.canvas.add(rectangle)
                start_x += x

            self.color_matrix.append(color_row_list)
            start_y -= y

        self.free_blocks = [tup for el in self.color_matrix for tup in el]
        shuffle(self.free_blocks)
        self.free_blocks = set(self.free_blocks)
        self.free_blocks.discard(0)  # del wall

    def __setup(self,source,count=5):
        data=[]
        for _ in range(count):
            shuffle(lst:=list(self.free_blocks))
            element = choice(lst)
            block = element[1]

            with self.canvas.after:
                rc = Rectangle(pos=block.pos,size=block.size,source = source)

            self.free_blocks.discard(element)
            data.append((rc.pos,rc))
        return data

    def setup_coins(self,count=5):
        coins=self.__setup('coin.png', count=count)
        self.coins_dct.update(dict(coins))

    def setup_monstors(self,count=5):
        monstors=self.__setup('monstor.png',count=count)
        for pos,mnstr in monstors:
            col,row = pos[0]//self.block_size[0],self.Y-1-pos[1]//self.block_size[1]
            self.monstors_dct[mnstr]=(int(row),int(col))

        self.event_monstor = Clock.schedule_interval(self._update_pos_monstors, 1)



    def setup_end(self):
        "Создаем блок выхода"
        end=choice([(row_ind,col_ind) for row_ind in range(self.Y-5,self.Y) for col_ind,col in enumerate(self.field[row_ind][-5:],self.X-5) if col==self.TRAP_SIGN])
        exit = self.color_matrix[end[0]][end[1]]
        with self.canvas.after:
            Rectangle(pos=exit[1].pos,size=exit[1].size,source='exit.webp')
        self.exit_coords = end
        return exit

    def _update_pos_monstors(self,*args):
        for mnstr in self.monstors_dct:
            trap = self._available_trap(self.monstors_dct[mnstr])
            mnstr.pos =  self.color_matrix[trap[0]][trap[1]][1].pos
            self.monstors_dct[mnstr] = trap

    def _available_trap(self,tup_ind):
        available = []
        row,col=tup_ind
        if self.checkPoint(row, col - 1, self.WALL_SIGN):
            available.append((row,col-1))
        if self.checkPoint(row, col + 1, self.WALL_SIGN):
            available.append((row,col+1))
        if self.checkPoint(row - 1, col, self.WALL_SIGN):
            available.append((row-1,col))
        if self.checkPoint(row + 1, col, self.WALL_SIGN):
            available.append((row+1,col))

        return choice(available)


class GameApp(App):

    def build(self):
        self.sm = ScreenManager(transition=FadeTransition())
        screen1 = Screen(name='game')
        screen2 = Screen(name='gameover')

        scroll = ScrollView()
        scroll.add_widget(Field())
        screen1.add_widget(scroll)
        self.label = Label(markup=True)
        screen2.add_widget(self.label)

        self.sm.add_widget(screen1)
        self.sm.add_widget(screen2)


        return self.sm


app=GameApp()
app.run()
