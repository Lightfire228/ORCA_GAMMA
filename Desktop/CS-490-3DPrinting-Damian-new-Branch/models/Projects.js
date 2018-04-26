var mongoose = require('mongoose')
var Schema = mongoose.Schema;

var projectSchema = new Schema({

  projectName: String,
  projectComments: String,
  engineerID: {
    type: Schema.ObjectId,
    ref: 'users'
  },
  clientsID: {
    type: Schema.ObjectId,
    ref: 'users'
  },
  materialID: {
    type: Schema.ObjectId,
    ref: 'materials'
  },

  fileOldName: String,
  fileNewName: String,
  filePath: String,
  fileSize: Number,
  fileMimeType: String,
  fileMd5: String,
  fileEncoding: String,


  fileVolumeCmCubed: Number,
  density: {
    type: String,
    enum: ['Solid', 'Sparse', 'Sparse Double Dense']
  },
  clientName: String,
  engineerName: String,
  engineerEmail: String,
  materialName: String,
  materialCost: Number,
  email: String,
  finalCost: Number,
  Density: String,
  datePosted: Date,

  archived: Boolean,
  completed: Boolean

});
module.exports = mongoose.model('project', projectSchema);
