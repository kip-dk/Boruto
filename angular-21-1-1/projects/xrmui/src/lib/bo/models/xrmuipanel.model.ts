export class XrmuiPanel {
    orientation: 'horizontal' | 'vertical' = 'horizontal';
    top: string = '';
    bottom: string = '';
    height: string = '';
    left: string = '';
    right: string = '';
    width: string = '';
    style: string = '';
    resizeStyle: string = '';
    dyn: boolean = false;
    index: number = 0;
    private initialWidth: number = 0;
    private initialHeight: number = 0;

    render(isresize: boolean = false) {
        this.style = '';
        this.add('top', this.top);
        this.add('bottom', this.bottom);
        this.add('height', this.height);
        this.add('left', this.left);
        this.add('right', this.right);
        this.add('width', this.width);

        if (!isresize && this.orientation == 'vertical') {
            this.resizeStyle = 'left: ' + this.width;
            this.initialWidth = Number(this.width.replace('px',''));
        }

        if (!isresize && this.orientation == 'horizontal') {
            this.resizeStyle = 'top: ' + this.height;
            this.initialHeight = Number(this.height.replace('px',''));
        }
    }

    private add(s: string, v: string) {
        if (v != '') {
            if (this.style.length > 0) {
                this.style += ";";
            }
            this.style += s + ": " + v;
        }
    }

    moveTo(pixel: number, next: XrmuiPanel) {

        if (this.orientation == 'vertical') {
        // vertical increase/decrease width on current
        // vertical increase/decrease left on next
            const newWidth =  this.initialWidth + pixel;
            this.width = newWidth + 'px';
            next.left = newWidth + 'px';
            this.render(true);
            next.render(true);
        }

        if (this.orientation == 'horizontal') {
            const newHeight =  this.initialHeight + pixel;
            this.height = newHeight + 'px';
            next.top = newHeight + 'px';
            this.render(true);
            next.render(true);
        }
    }

    endResize() {
        if (this.orientation == 'vertical') {
            this.initialWidth = Number(this.width.replace('px',''));
        }

        if (this.orientation == 'horizontal') {
            this.initialHeight = Number(this.height.replace('px',''));
        }
    }
}
