﻿using Melanchall.DryWetMidi.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Melanchall.DryWetMidi.Smf.Interaction
{
    /// <summary>
    /// Extension methods for chords managing.
    /// </summary>
    public static class ChordsManagingUtilities
    {
        #region Methods

        /// <summary>
        /// Creates an instance of the <see cref="ChordsManager"/> initializing it with the
        /// specified events collection, notes tolerance and comparison delegate for events that have same time.
        /// </summary>
        /// <param name="eventsCollection"><see cref="EventsCollection"/> that holds chords to manage.</param>
        /// <param name="notesTolerance">Notes tolerance that defines maximum distance of notes from the
        /// start of the first note of a chord. Notes within this tolerance will be considered as a chord.</param>
        /// <param name="sameTimeEventsComparison">Delegate to compare events with the same absolute time.</param>
        /// <returns>An instance of the <see cref="ChordsManager"/> that can be used to manage chords
        /// represented by the <paramref name="eventsCollection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="eventsCollection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notesTolerance"/> is negative.</exception>
        public static ChordsManager ManageChords(this EventsCollection eventsCollection, long notesTolerance = 0, Comparison<MidiEvent> sameTimeEventsComparison = null)
        {
            ThrowIf.ArgumentIsNull(nameof(eventsCollection), eventsCollection);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            return new ChordsManager(eventsCollection, notesTolerance, sameTimeEventsComparison);
        }

        /// <summary>
        /// Creates an instance of the <see cref="ChordsManager"/> initializing it with the
        /// events collection of the specified track chunk, notes tolerance and comparison delegate for events
        /// that have same time.
        /// </summary>
        /// <param name="trackChunk"><see cref="TrackChunk"/> that holds chords to manage.</param>
        /// <param name="notesTolerance">Notes tolerance that defines maximum distance of notes from the
        /// start of the first note of a chord. Notes within this tolerance will be considered as a chord.</param>
        /// <param name="sameTimeEventsComparison">Delegate to compare events with the same absolute time.</param>
        /// <returns>An instance of the <see cref="ChordsManager"/> that can be used to manage
        /// chords represented by the <paramref name="trackChunk"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="trackChunk"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notesTolerance"/> is negative.</exception>
        public static ChordsManager ManageChords(this TrackChunk trackChunk, long notesTolerance = 0, Comparison<MidiEvent> sameTimeEventsComparison = null)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunk), trackChunk);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            return trackChunk.Events.ManageChords(notesTolerance, sameTimeEventsComparison);
        }

        public static IEnumerable<Chord> GetChords(this EventsCollection eventsCollection, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(eventsCollection), eventsCollection);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            return eventsCollection.ManageChords(notesTolerance).Chords;
        }

        /// <summary>
        /// Gets chords contained in the specified track chunk.
        /// </summary>
        /// <param name="trackChunk"><see cref="TrackChunk"/> to search for chords.</param>
        /// <param name="notesTolerance">Notes tolerance that defines maximum distance of notes from the
        /// start of the first note of a chord. Notes within this tolerance will be considered as a chord.</param>
        /// <returns>Collection of chords contained in <paramref name="trackChunk"/> ordered by time.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="trackChunk"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notesTolerance"/> is negative.</exception>
        public static IEnumerable<Chord> GetChords(this TrackChunk trackChunk, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunk), trackChunk);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            return trackChunk.Events.GetChords(notesTolerance);
        }

        /// <summary>
        /// Gets chords contained in the specified track chunks.
        /// </summary>
        /// <param name="trackChunks">Track chunks to search for chords.</param>
        /// <param name="notesTolerance">Notes tolerance that defines maximum distance of notes from the
        /// start of the first note of a chord. Notes within this tolerance will be considered as a chord.</param>
        /// <returns>Collection of chords contained in <paramref name="trackChunks"/> ordered by time.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="trackChunks"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notesTolerance"/> is negative.</exception>
        public static IEnumerable<Chord> GetChords(this IEnumerable<TrackChunk> trackChunks, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunks), trackChunks);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            return trackChunks.Where(c => c != null)
                              .SelectMany(c => c.GetChords(notesTolerance))
                              .OrderBy(c => c.Time);
        }

        /// <summary>
        /// Gets chords contained in the specified MIDI file.
        /// </summary>
        /// <param name="file"><see cref="MidiFile"/> to search for chords.</param>
        /// <param name="notesTolerance">Notes tolerance that defines maximum distance of notes from the
        /// start of the first note of a chord. Notes within this tolerance will be considered as a chord.</param>
        /// <returns>Collection of notes contained in <paramref name="file"/> ordered by time.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notesTolerance"/> is negative.</exception>
        public static IEnumerable<Chord> GetChords(this MidiFile file, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(file), file);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            return file.GetTrackChunks().GetChords(notesTolerance);
        }

        public static void ProcessChords(this EventsCollection eventsCollection, Action<Chord> action, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(eventsCollection), eventsCollection);
            ThrowIf.ArgumentIsNull(nameof(action), action);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            using (var chordsManager = eventsCollection.ManageChords(notesTolerance))
            {
                foreach (var chord in chordsManager.Chords.Where(c => match?.Invoke(c) != false))
                {
                    action(chord);
                }
            }
        }

        public static void ProcessChords(this TrackChunk trackChunk, Action<Chord> action, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunk), trackChunk);

            trackChunk.Events.ProcessChords(action, match, notesTolerance);
        }

        public static void ProcessChords(this IEnumerable<TrackChunk> trackChunks, Action<Chord> action, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunks), trackChunks);

            foreach (var trackChunk in trackChunks)
            {
                trackChunk?.ProcessChords(action, match, notesTolerance);
            }
        }

        public static void ProcessChords(this MidiFile file, Action<Chord> action, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(file), file);

            file.GetTrackChunks().ProcessChords(action, match, notesTolerance);
        }

        public static void RemoveChords(this EventsCollection eventsCollection, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(eventsCollection), eventsCollection);

            if (notesTolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(notesTolerance), notesTolerance, "Notes tolerance is negative.");

            using (var chordsManager = eventsCollection.ManageChords(notesTolerance))
            {
                chordsManager.Chords.RemoveAll(match ?? (c => true));
            }
        }

        public static void RemoveChords(this TrackChunk trackChunk, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunk), trackChunk);

            trackChunk.Events.RemoveChords(match, notesTolerance);
        }

        public static void RemoveChords(this IEnumerable<TrackChunk> trackChunks, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(trackChunks), trackChunks);

            foreach (var trackChunk in trackChunks)
            {
                trackChunk?.RemoveChords(match, notesTolerance);
            }
        }

        public static void RemoveChords(this MidiFile file, Predicate<Chord> match = null, long notesTolerance = 0)
        {
            ThrowIf.ArgumentIsNull(nameof(file), file);

            file.GetTrackChunks().RemoveChords(match, notesTolerance);
        }

        #endregion
    }
}