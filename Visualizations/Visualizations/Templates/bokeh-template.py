
import locale
# Required to prevent error message 'Python.Runtime.PythonException: 'unknown locale: en-US'' when starting VisuAlFroG via Grasshopper
locale.setlocale(locale.LC_ALL, "C")

import os
import sys

from bokeh.models import ColumnDataSource, Patches, CustomJS
from bokeh.plotting import figure, show
from bokeh.layouts import row
from bokeh.io import output_file

import pandas as pd

import clr
clr.AddReference('Visualizations')
from Visualizations.PythonInterface import PythonCallback



def Start():
    PythonCallback.PrintMessage('Hello')

print("Python Version: " + sys.version)

output_file(PythonCallback.GetBokehOutputFilePath())

x = [[1,2,4], [3,5,6], [7,9,7], [5,7,6]]
y = [[4,2,1], [6,5,8], [3,9,6], [2,2,1]]
group = ['A', 'A', 'B', 'B']
id = [0,1,2,3]

df = pd.DataFrame(data=dict(x=x, y=y, group=group, id=id))
source = ColumnDataSource(df)

p = figure(tools="tap")

renderer = p.patches('x', 'y', source=source)

def my_tap_handler(attr,old,new):
    print("Python Handler called")
    indices = source.selected.indices
    if len(indices) == 1:
        group = source.data["group"][indices[0]]
        new_indices = [i for i, g in enumerate(source.data["group"]) if g == group]
        if new_indices != indices:
            source.selected = Selection(indices=new_indices)


callback = CustomJS(args=dict(source=source), code="""
    console.log('JavaScript Handler Called ...');
    const data = source.data;
    const idcs = cb_obj.indices;
    console.log(typeof idcs);
    if (idcs.length == 1) {
        const group = source.data["group"][idcs[0]];
        var new_indices = [];
        for (const [index, element] of source.data["group"].entries()) {
            if (element == group) {
                new_indices.push(index);
            }
        }
        if (new_indices != idcs) {
            console.log(new_indices);
            source.selected.indices = new_indices;
        }
    }

    source.change.emit();
""")

selected_patches = Patches(fill_color="#a6cee3")
renderer.selection_glyph = selected_patches
source.selected.js_on_change('indices', callback)

print("Show Plot")
show(p, width=800)
