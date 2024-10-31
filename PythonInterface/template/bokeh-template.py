
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

output_file(PythonCallback.GetOutputFile())

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
    indexes = source.selected.indexes
    if len(indexes) == 1:
        group = source.data["group"][indexes[0]]
        new_indexes = [i for i, g in enumerate(source.data["group"]) if g == group]
        if new_indexes != indexes:
            source.selected = Selection(indexes=new_indexes)


callback = CustomJS(args=dict(source=source), code="""
    console.log('JavaScript Handler Called ...');
    const data = source.data;
    const idcs = cb_obj.indexes;
    console.log(typeof idcs);
    if (idcs.length == 1) {
        const group = source.data["group"][idcs[0]];
        var new_indexes = [];
        for (const [index, element] of source.data["group"].entries()) {
            if (element == group) {
                new_indexes.push(index);
            }
        }
        if (new_indexes != idcs) {
            console.log(new_indexes);
            source.selected.indexes = new_indexes;
        }
    }

    source.change.emit();
""")

selected_patches = Patches(fill_color="#a6cee3")
renderer.selection_glyph = selected_patches
source.selected.js_on_change('indexes', callback)

print("Show Plot")
show(p, width=800)
