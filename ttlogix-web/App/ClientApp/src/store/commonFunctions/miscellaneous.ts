import moment from 'moment';

// Date
export function showDate(date:string, showTime: boolean = false){
    return date ? moment(date).format('DD/MM/YYYY' + (showTime ? ' HH:mm' : '')) : '';
}

// Dictionaty
export type Dictionary<T> = { [key: string]: T };

// From JSON
export function fromJSON<T extends {}>(ctor: new() => T, json:any){
    const d = new ctor();
    Object.assign(d,json);
    return d;
}

// Scroll sideways
export function scrollSideways(elementId:string){
    const element = document.getElementById(elementId);
    const inputs = element?.getElementsByTagName('input');
    const selects = element?.getElementsByTagName('select');
    let isDown:boolean = false; let startX:number; let scrollLeft:number;
    if(element){
        element.addEventListener('mousedown', (e) => {
            isDown = true;
            startX = e.pageX - element.offsetLeft;
            scrollLeft = element.scrollLeft;
        });

        element.addEventListener('mouseleave', () => { isDown = false; });

        element.addEventListener('mouseup', () => { isDown = false; });

        element.addEventListener('mousemove', (e) => {
            if(!isDown) return;
            e.preventDefault();
            const x = e.pageX - element.offsetLeft;
            const walk = (x - startX) * 2;
            element.scrollLeft = scrollLeft - walk;
        });

        if(inputs){
            for(var i = 0; i < inputs.length; i++) {
                inputs[i].addEventListener('mousedown', (e) => {
                    if(inputs[i]) inputs[i].select();
                    e.stopPropagation();
                    return;
                });
            }
        }

        if(selects){
            for(var s = 0; s < selects.length; s++) {
                selects[s].addEventListener('mousedown', (e) => {
                    e.stopPropagation(); return;
                });
            }
        }
    }
}

